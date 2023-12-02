using Mediator;
using Microsoft.EntityFrameworkCore;
using Mux.Csharp.Sdk.Model;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using Error = OneOf.Types.Error;
using LiveStream = ZulaMed.API.Domain.LiveStream.LiveStream;

namespace ZulaMed.API.Endpoints.MuxWebhook;

public interface IMuxEvent : IRequest<OneOf<Success, Error>>
{
    public MuxData Data { get; set; }
}

public interface IMuxEventHandler<in T> : IRequestHandler<T, OneOf<Success, Error>> where T : IMuxEvent
{
}

public class AssetCreatedEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
    public required string Type { get; set; }
}

public class AssetCreatedEventHandler : IMuxEventHandler<AssetCreatedEvent>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly ILogger<AssetCreatedEventHandler> _logger;

    public AssetCreatedEventHandler(ZulaMedDbContext dbContext, ILogger<AssetCreatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async ValueTask<OneOf<Success, Error>> Handle(AssetCreatedEvent request, CancellationToken cancellationToken)
    {
        var asset = request.Data.Data.AsT0;
        if (asset.Status == Asset.StatusEnum.Ready)
        {
            _logger.LogInformation("Asset {Id} is ready, no reason for video.asset.created", asset.Id);
            return new Error();
        }

        try
        {
            var rows = await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.Data.Metadata.VideoId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.VideoStatus, VideoStatus.GettingProcessed),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}

public class AssetReadyEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class AssetReadyEventHandler : IMuxEventHandler<AssetReadyEvent>
{
    private readonly ZulaMedDbContext _dbContext;

    public AssetReadyEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(AssetReadyEvent request,
        CancellationToken cancellationToken)
    {
        var asset = request.Data.Data.AsT0;
        try
        {
            if (asset.IsLive)
            {
                return new Success();
            }
            var playbackId = asset.PlaybackIds.First().Id;
            var videoUrl = VideoUrl.From($"https://stream.mux.com/{playbackId}.m3u8");
            var timeLineThumbnail = VideoTimelineThumbnail.From($"https://image.mux.com/{playbackId}/storyboard.vtt");
            var thumbnail = VideoThumbnail.From($"https://image.mux.com/{playbackId}/thumbnail.png");
            await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.Data.Metadata.VideoId)
                .ExecuteUpdateAsync(calls => calls
                        .SetProperty(x => x.VideoStatus, VideoStatus.Ready)
                        .SetProperty(x => x.VideoUrl, videoUrl)
                        .SetProperty(x => x.VideoThumbnail, thumbnail)
                        .SetProperty(x => x.VideoTimelineThumbnail, timeLineThumbnail),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}

public class UploadCanceledEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class UploadCanceledEventHandler : IMuxEventHandler<UploadCanceledEvent>
{
    private readonly ZulaMedDbContext _dbContext;

    public UploadCanceledEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(UploadCanceledEvent request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Set<Video>()
                .Where(x => (Guid)x.Id == request.Data.Metadata.VideoId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.VideoStatus, VideoStatus.Cancelled),
                    cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception)
        {
            return new Error();
        }
    }
}

public class LiveStreamActiveEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class LiveStreamActiveEventHandler : IMuxEventHandler<LiveStreamActiveEvent>
{
    private readonly ZulaMedDbContext _dbContext;

    public LiveStreamActiveEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(LiveStreamActiveEvent request,
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Set<LiveStream>().Where(x => (Guid)x.Id == request.Data.Metadata.LiveStreamId)
                .ExecuteUpdateAsync(calls =>
                        calls.SetProperty(x => x.Status, LiveStreamStatus.Active)
                    , cancellationToken: cancellationToken);
            return new Success();
        }
        catch (Exception e)
        {
            return new Error();
        }
    }
}

public class AssetLiveStreamCompletedEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class AssetLiveStreamCompletedEventHandler : IMuxEventHandler<AssetLiveStreamCompletedEvent>
{
    private readonly ZulaMedDbContext _dbContext;

    public AssetLiveStreamCompletedEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(AssetLiveStreamCompletedEvent request,
        CancellationToken cancellationToken)
    {
        var asset = request.Data.Data.AsT0;
        try
        {
            var playbackId = asset.PlaybackIds.First().Id;
            var videoUrl = VideoUrl.From($"https://stream.mux.com/{playbackId}.m3u8");
            var timeLineThumbnail = VideoTimelineThumbnail.From($"https://image.mux.com/{playbackId}/storyboard.vtt");
            var thumbnail = VideoThumbnail.From($"https://image.mux.com/{playbackId}/thumbnail.png");

            var rows = await _dbContext.Set<Video>().Where(x => (Guid)x.Id == request.Data.Metadata.VideoId)
                .ExecuteUpdateAsync(calls => calls
                    .SetProperty(x => x.VideoStatus, VideoStatus.Ready)
                    .SetProperty(x => x.VideoUrl, videoUrl)
                    .SetProperty(x => x.VideoThumbnail, thumbnail)
                    .SetProperty(x => x.VideoTimelineThumbnail, timeLineThumbnail), cancellationToken);
        }
        catch (Exception e)
        {
            return new Error();
        }

        return new Success();
    }
}