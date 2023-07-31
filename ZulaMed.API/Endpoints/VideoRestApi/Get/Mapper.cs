using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public static class Mapper 
{
    public static VideoDTO ToResponse(this Video video, string baseUrl)
    {
        return new VideoDTO
        {
            Id = video.Id.Value,
            VideoDescription = video.VideoDescription.Value,
            VideoThumbnail = $"{baseUrl}{video.VideoThumbnail.Value}",
            VideoUrl = video.VideoUrl.Value,
            VideoTitle = video.VideoTitle.Value,
            VideoPublishedDate = video.VideoPublishedDate.Value,
            VideoPublisherId = video.Publisher.Id.Value
        };
    }
} 