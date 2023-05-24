namespace ZulaMed.API.Endpoints.Video.Post;


public static class Mapper
{
    public static CreateVideoCommand MapToCommand(this Request request)
    {
        return new CreateVideoCommand
        {
            VideoTitle = request.VideoTitle,
            VideoUrl = request.VideoUrl,
            VideoThumbnail = request.VideoThumbnail,
            VideoDescription = request.VideoDescription,
            VideoPublishedDate = request.VideoPublishedDate
        };
    }

    public static VideoDTO MapToResponse(this Domain.Video.Video video)
    {
        return new VideoDTO
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
