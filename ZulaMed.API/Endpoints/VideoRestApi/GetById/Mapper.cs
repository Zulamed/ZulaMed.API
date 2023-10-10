using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public static class Mapper
{
    public static VideoDTO ToResponse(this Video video)
    {
        return new VideoDTO
        {
            Id = video.Id.Value,
            VideoDescription = video.VideoDescription?.Value,
            VideoThumbnail = video.VideoThumbnail?.Value,
            VideoUrl = video.VideoUrl?.Value,
            VideoTitle = video.VideoTitle?.Value,
            VideoPublishedDate = video.VideoPublishedDate.Value,
            VideoPublisherId = video.Publisher.Id.Value,
            VideoViews = video.VideoView.Value,
            VideoTimelineThumbnail = video.VideoTimelineThumbnail?.Value,
        };
    }
}