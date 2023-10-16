using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using Error = OneOf.Types.Error;

namespace ZulaMed.API.Endpoints.MuxWebhook;

public interface IMuxEvent : IRequest<OneOf<Success, Error>> 
{
    public MuxData Data { get; set; }
}



public class VideoAssetCreatedEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class VideoAssetCreatedEventHandler : IRequestHandler<VideoAssetCreatedEvent, OneOf<Success, Error>>
{
    private readonly ZulaMedDbContext _dbContext;

    public VideoAssetCreatedEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Success, Error>> Handle(VideoAssetCreatedEvent request, CancellationToken cancellationToken)
    {
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

public class VideoAssetReadyEvent : IMuxEvent
{
    public required MuxData Data { get; set; }
}

public class VideoAssetReadyEventHandler : IRequestHandler<VideoAssetReadyEvent, OneOf<Success, Error>>
{
    private readonly ZulaMedDbContext _dbContext;

    public VideoAssetReadyEventHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error>> Handle(VideoAssetReadyEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var playbackId = request.Data.Data.PlaybackIds.First().Id;
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

public class UploadCanceledEventHandler : IRequestHandler<UploadCanceledEvent, OneOf<Success, Error>>
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