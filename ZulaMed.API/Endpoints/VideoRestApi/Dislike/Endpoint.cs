using System.Runtime.InteropServices;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Dislike;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Dislike;
using Request = ZulaMed.API.Endpoints.VideoRestApi.Like.Request;

namespace ZulaMed.API.Endpoints.VideoRestApi.Dislike;

public class DislikeVideoCommand : Mediator.ICommand<OneOf<Success, UserAlreadyDisliked, Error<string>, NotFound>>
{
    public required Guid VideoId { get; init; }

    public required Guid UserId { get; init; }
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UserDidNotDislike
{
}
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UserAlreadyDisliked
{
}
public class UnDislikeVideoCommand : Mediator.ICommand<OneOf<Success, UserDidNotDislike, Error<string>, NotFound>>
{
    public required Guid VideoId { get; init; }

    public required Guid UserId { get; init; }
}

public class
    DislikeVideoCommandHandler : Mediator.ICommandHandler<DislikeVideoCommand,
        OneOf<Success, UserAlreadyDisliked, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public DislikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Success, UserAlreadyDisliked, Error<string>, NotFound>> Handle(DislikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userAlreadyDisliked = await _dbContext.Set<Dislike<Video>>()
                .AnyAsync(x => (Guid)x.DislikedBy.Id == command.UserId && (Guid)x.Parent.Id == command.VideoId,
                    cancellationToken: cancellationToken);
            if (userAlreadyDisliked)
            {
                return new UserAlreadyDisliked();
            }

            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoDislike" = "VideoDislike" + 1 WHERE "Id" = {command.VideoId}""",
                cancellationToken: cancellationToken);
            await _dbContext.Database.ExecuteSqlAsync
            ($"""INSERT INTO "Dislike<Video>" VALUES ({Guid.NewGuid()}, {command.VideoId}, {command.UserId}, {DateTime.UtcNow})""",
                cancellationToken: cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new Error<string>(e.Message);
        }
    }
}

public class
    UnDislikeVideoCommandHandler : Mediator.ICommandHandler<UnDislikeVideoCommand,
        OneOf<Success, UserDidNotDislike, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UnDislikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, UserDidNotDislike, Error<string>, NotFound>> Handle(UnDislikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var didUserDislike = await _dbContext.Set<Dislike<Video>>()
                .AnyAsync(x => (Guid)x.DislikedBy.Id == command.UserId && (Guid)x.Parent.Id == command.VideoId,
                    cancellationToken: cancellationToken);
            if (!didUserDislike)
            {
                return new UserDidNotDislike();
            }

            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoDislike" = GREATEST("VideoDislike" - 1, 0) WHERE "Id" = {command.VideoId}""",
                cancellationToken: cancellationToken);
            await _dbContext.Database.ExecuteSqlAsync(
                $"""DELETE FROM "Dislike<Video>" WHERE "ParentId" = {command.VideoId} AND "UserId" = {command.UserId}""",
                cancellationToken: cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new Error<string>(e.Message);
        }
    }
}
public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Routes("/video/{id}/dislike");
        Verbs(Http.POST, Http.DELETE);
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // getting authenticated user id
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        if (HttpContext.Request.Method == Http.POST.ToString())
        {
            var result = await _mediator.Send(new DislikeVideoCommand { VideoId = req.Id, UserId = userId }, ct);
            await result.Match(
                s => SendOkAsync(ct),
                ua =>
                {
                    AddError("User already disliked this video");
                    return SendErrorsAsync(cancellation: ct);
                },
                e => SendAsync(new
                {
                    StatusCode = 500,
                    Message = e.Value
                }, 500, ct),
                n => SendNotFoundAsync(ct)
            );
        }
        else if (HttpContext.Request.Method == Http.DELETE.ToString())
        {
            var result = await _mediator.Send(new UnDislikeVideoCommand { VideoId = req.Id, UserId = userId }, ct);
            await result.Match(
                s => SendOkAsync(ct),
                ue =>
                {
                    AddError("User did not dislike this video");
                    return SendErrorsAsync(cancellation: ct);
                },
                e => SendAsync(new
                {
                    StatusCode = 500,
                    Message = e.Value
                }, 500, ct),
                n => SendNotFoundAsync(ct)
            );
        }
    }
}