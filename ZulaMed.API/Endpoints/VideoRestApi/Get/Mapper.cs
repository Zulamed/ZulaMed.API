using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public static class Mapper
{
    public static AllVideoDto ToResponse(this Video video, string baseUrl)
    {
        var videoDto = new VideoDTO
        {
            Id = video.Id.Value,
            VideoDescription = video.VideoDescription?.Value,
            VideoThumbnail = $"{baseUrl}{video.VideoThumbnail?.Value}",
            VideoUrl = video.VideoUrl?.Value,
            VideoTitle = video.VideoTitle?.Value,
            VideoPublishedDate = video.VideoPublishedDate.Value,
            VideoPublisherId = video.Publisher.Id.Value,
            VideoViews = video.VideoView.Value
        };

        var profilePicture = video.Publisher.PhotoUrl?.Value == null ? null : $"{baseUrl}{video.Publisher.PhotoUrl.Value}";
        var userDto = new UserDTO
        {
            Id = video.Publisher.Id.Value,
            ProfilePictureUrl = profilePicture,
            Username = video.Publisher.Login.Value
        };
        return new AllVideoDto()
        {
            User = userDto,
            Video = videoDto
        };
    }
}