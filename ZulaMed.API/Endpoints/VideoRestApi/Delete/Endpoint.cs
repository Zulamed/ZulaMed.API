using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Mux.Csharp.Sdk.Api;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Delete;

public class DeleteVideoCommandHandler : Mediator.ICommandHandler<DeleteVideoCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly AssetsApi _assetsApi;
    private readonly PlaybackIDApi _playbackIdApi;

    public DeleteVideoCommandHandler(ZulaMedDbContext dbContext, AssetsApi assetsApi, PlaybackIDApi playbackIdApi)
    {
        _dbContext = dbContext;
        _assetsApi = assetsApi;
        _playbackIdApi = playbackIdApi;
    }


    private string GetPlaybackId(string playbackId)
    {
        var playbackIdParts = playbackId.Split("/");
        return playbackIdParts[^1].Replace(".m3u8", "");
    }

    public async ValueTask<bool> Handle(DeleteVideoCommand command, CancellationToken cancellationToken)
    {
        var video = await _dbContext.Set<Video>().SingleOrDefaultAsync(
            x => command.Id == (Guid)x.Id && command.UserId == (Guid)x.Publisher.Id,
            cancellationToken: cancellationToken);

        if (video == null)
        {
            return false;
        }

        if (video.VideoUrl is not null)
        {
            try
            {
                var asset = await _playbackIdApi.GetAssetOrLivestreamIdAsync(GetPlaybackId(video.VideoUrl.Value),
                    cancellationToken);
                await _assetsApi.DeleteAssetAsync(asset.Data.Object.Id, cancellationToken);
            }
            catch
            {
                // ignored
            }
        }

        _dbContext.Set<Video>().Remove(video);

        await _dbContext.SaveChangesAsync(cancellationToken);


        return true;
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
        Delete("/video/{id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new DeleteVideoCommand
        {
            Id = req.Id,
            UserId = userId
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
}