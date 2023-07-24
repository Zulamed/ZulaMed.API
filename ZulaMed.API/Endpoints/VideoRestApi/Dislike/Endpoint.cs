using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;

namespace ZulaMed.API.Endpoints.VideoRestApi.Dislike;

public record DislikeVideoCommand(Guid Id)
    : Mediator.ICommand<OneOf<Success, NotFound, Error<string>>>;
public record UnDislikeVideoCommand(Guid Id)
    : Mediator.ICommand<OneOf<Success, NotFound, Error<string>>>;
public class
    DislikeVideoCommandHandler : Mediator.ICommandHandler<DislikeVideoCommand, OneOf<Success, NotFound, Error<string>>>
{
    private readonly ZulaMedDbContext _dbContext;

    public DislikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, NotFound, Error<string>>> Handle(DislikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoDislike" = "VideoDislike" + 1 WHERE "Id" = {command.Id} """,
                cancellationToken);
            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
            return new Error<string>(e.Message);
        }
    }
}
public class
    UnDislikeVideoCommandHandler : Mediator.ICommandHandler<UnDislikeVideoCommand, OneOf<Success, NotFound, Error<string>>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UnDislikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, NotFound, Error<string>>> Handle(UnDislikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoDislike" = GREATEST("VideoDislike" - 1, 0) WHERE "Id" = {command.Id} """,
                cancellationToken);
            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
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
        Verbs(Http.POST, Http.DELETE);
        Routes("/video/{id}/dislike", "/video/{id}/undislike");
        AllowAnonymous();
        Description(b => {}, clearDefaults: true);
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (HttpContext.Request.Method == Http.POST.ToString())
        {
            var result = await _mediator.Send(new DislikeVideoCommand(req.Id), ct);
            await result.Match(
                success => SendOkAsync(ct),
                notFound => SendNotFoundAsync(ct),
                error => SendAsync(new
                {
                    StatusCode = 500,
                    Message = error.Value
                }, 500, ct) 
            );
        }
        else if (HttpContext.Request.Method == Http.DELETE.ToString())
        {
            var result = await _mediator.Send(new UnDislikeVideoCommand(req.Id), ct);
            await result.Match(
                success => SendOkAsync(ct),
                notFound => SendNotFoundAsync(ct),
                error => SendAsync(new
                {
                    StatusCode = 500,
                    Message = error.Value
                }, 500, ct) 
            );
        }
        
        
        
        
    }
}