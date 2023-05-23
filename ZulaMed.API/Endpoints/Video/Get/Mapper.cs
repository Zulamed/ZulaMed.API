namespace ZulaMed.API.Endpoints.Video.Get;

public static class Mapper 
{
    public static VideoDTO ToResponse(this Domain.Video.Video video)
    {
        return new VideoDTO()
        {
            Id = video.Id.Value,
            VideoDescription = video.VideoDescription.Value,
            VideoThumbnail = video.VideoThumbnail.Value,
            VideoUrl = video.VideoUrl.Value,
            VideoTitle = video.VideoTitle.Value,
            VideoPublishedDate = video.VideoPublishedDate.Value
        };
    }
} 