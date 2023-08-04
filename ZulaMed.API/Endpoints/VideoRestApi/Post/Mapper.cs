using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Post;


public static class Mapper
{
    public static CreateVideoCommand MapToCommand(this Request request)
    {
        return new CreateVideoCommand
        {
            VideoTitle = request.VideoTitle,
            VideoPublisherId = request.VideoPublisherId,
            VideoThumbnail = request.VideoThumbnail,
            VideoDescription = request.VideoDescription,
            Video = request.Video,
        };
    }

    public static VideoDTO MapToResponse(this Video video)
    {
        return new VideoDTO
        {
            Id = video.Id.Value,
            VideoPublisherId = video.Publisher.Id.Value,
            VideoDescription = video.VideoDescription.Value,
            VideoThumbnail = video.VideoThumbnail.Value,
            VideoUrl = video.VideoUrl.Value,
            VideoTitle = video.VideoTitle.Value,
            VideoPublishedDate = video.VideoPublishedDate.Value,
            VideoViews = video.VideoView.Value
        };
    }
}
