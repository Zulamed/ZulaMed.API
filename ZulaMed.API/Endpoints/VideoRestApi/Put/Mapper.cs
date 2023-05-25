using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public static class Mapper
{
    public static UpdateVideoCommand MapToCommand(this Request request)
    {
        return new UpdateVideoCommand
        {
            VideoTitle = request.VideoTitle,
            VideoThumbnail = request.VideoThumbnail,
            VideoDescription = request.VideoDescription
        };
    } 
    
    public static VideoDTO MapToResponse(this Video video)
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