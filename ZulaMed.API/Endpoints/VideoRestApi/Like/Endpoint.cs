using System.Runtime.InteropServices;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Like;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Like;

public class LikeVideoCommand : Mediator.ICommand<OneOf<Success, UserAlreadyLiked, Error<string>, NotFound>>
{
    public required Guid VideoId { get; init; }

    public required Guid UserId { get; init; }
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UserDidNotLike
{
}

public class UnLikeVideoCommand : Mediator.ICommand<OneOf<Success, UserDidNotLike, Error<string>, NotFound>>
{
    public required Guid VideoId { get; init; }

    public required Guid UserId { get; init; }
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UserAlreadyLiked
{
}

public class
    LikeVideoCommandHandler : Mediator.ICommandHandler<LikeVideoCommand,
        OneOf<Success, UserAlreadyLiked, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public LikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Success, UserAlreadyLiked, Error<string>, NotFound>> Handle(LikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userAlreadyLiked = await _dbContext.Set<Like<Video>>()
                .AnyAsync(x => (Guid)x.LikedBy.Id == command.UserId && (Guid)x.Parent.Id == command.VideoId,
                    cancellationToken: cancellationToken);
            if (userAlreadyLiked)
            {
                return new UserAlreadyLiked();
            }

            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoLike" = "VideoLike" + 1 WHERE "Id" = {command.VideoId}""",
                cancellationToken: cancellationToken);
            await _dbContext.Database.ExecuteSqlAsync
            ($"""INSERT INTO "LikeVideo" VALUES ({Guid.NewGuid()}, {DateTime.UtcNow}, {command.VideoId}, {command.UserId})""",
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
    UnLikeVideoCommandHandler : Mediator.ICommandHandler<UnLikeVideoCommand,
        OneOf<Success, UserDidNotLike, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UnLikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, UserDidNotLike, Error<string>, NotFound>> Handle(UnLikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var didUserLike = await _dbContext.Set<Like<Video>>()
                .AnyAsync(x => (Guid)x.LikedBy.Id == command.UserId && (Guid)x.Parent.Id == command.VideoId,
                    cancellationToken: cancellationToken);
            if (!didUserLike)
            {
                return new UserDidNotLike();
            }

            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoLike" = GREATEST("VideoLike" - 1, 0) WHERE "Id" = {command.VideoId}""",
                cancellationToken: cancellationToken);
            await _dbContext.Database.ExecuteSqlAsync(
                $"""DELETE FROM "LikeVideo" WHERE "ParentId" = {command.VideoId} AND "UserId" = {command.UserId}""",
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
        Routes("/video/{id}/like");
        Verbs(Http.POST, Http.DELETE);
        AllowAnonymous();
        // for some reason FastEndpoints was sending 415, this clears the defaults so that it wouldn't send it 
        Description(b => { }, clearDefaults: true);
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (HttpContext.Request.Method == Http.POST.ToString())
        {
            var result = await _mediator.Send(new LikeVideoCommand { VideoId = req.Id, UserId = req.UserId }, ct);
            await result.Match(
                s => SendOkAsync(ct),
                ua =>
                {
                    AddError("User already liked this video");
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
            var result = await _mediator.Send(new UnLikeVideoCommand { VideoId = req.Id, UserId = req.Id }, ct);
            await result.Match(
                s => SendOkAsync(ct),
                ue =>
                {
                    AddError("User did not like this video");
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