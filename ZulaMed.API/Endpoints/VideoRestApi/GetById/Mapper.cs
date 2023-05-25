using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public static class Mapper
{
    public static Response ToResponse(this Video video)
    {
        return new Response()
        {
            Video = new VideoDTO()
            {
                Id = video.Id.Value,
                VideoDescription = video.VideoDescription.Value,
                VideoThumbnail = video.VideoThumbnail.Value,
                VideoUrl = video.VideoUrl.Value,
                VideoTitle = video.VideoTitle.Value,
                VideoPublishedDate = video.VideoPublishedDate.Value
            }
        };
    }
}