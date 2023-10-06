using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public static class Mapper
{
    public static UpdateVideoCommand MapToCommand(this Request request)
    {
        return new UpdateVideoCommand
        {
            Id = request.Id,
            VideoTitle = request.VideoTitle,
            VideoThumbnail = request.VideoThumbnail,
            VideoDescription = request.VideoDescription,
            VideoUrl = request.VideoUrl,
        };
    } 
}