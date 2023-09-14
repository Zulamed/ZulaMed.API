using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Domain.ViewHistory;

namespace ZulaMed.API.Endpoints.VideoRestApi.View;

public class ViewCommandHandler : Mediator.ICommandHandler<ViewCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public ViewCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(ViewCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var video = await _dbContext.Set<Video>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.Id, cancellationToken);
            if (video is null)
            {
                return new NotFound();
            }

            if (command.WatchedBy is not null)
            {
                var row = await _dbContext.Database.ExecuteSqlAsync(
                    $"""UPDATE "ViewHistory" SET "ViewedAt" = {DateTime.UtcNow} WHERE "ViewedById" = {command.WatchedBy.Value} AND "ViewedVideoId" = {video.Id.Value}""",
                    cancellationToken: cancellationToken);
                if (row == 0)
                {
                    await _dbContext.Database.ExecuteSqlAsync
                    ($"""INSERT INTO "ViewHistory" VALUES ({Guid.NewGuid()}, {DateTime.UtcNow}, {command.Id}, {command.WatchedBy.Value})""",
                        cancellationToken: cancellationToken);
                }
            }

            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoView" = "VideoView" + 1 WHERE "Id" = {command.Id}""",
                cancellationToken: cancellationToken);
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
        Post("video/{id}/{watchedBy}/view");
        AllowAnonymous();
        Description(b => { }, clearDefaults: true);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new ViewCommand { Id = req.Id, WatchedBy = req.WatchedBy }, ct);
        await result.Match(
            s => SendOkAsync(ct),
            e => SendAsync(new
            {
                StatusCode = 500,
                Message = e.Value
            }, 500, ct),
            n => SendNotFoundAsync(ct)
        );
    }
}