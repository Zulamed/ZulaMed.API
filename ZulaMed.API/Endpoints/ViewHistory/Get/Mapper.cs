namespace ZulaMed.API.Endpoints.ViewHistory.Get;

public static class Mapper
{
    public static ViewHistoryDTO ToResponse(this Domain.ViewHistory.ViewHistory viewHistory)
    {
        return new ViewHistoryDTO
        {
            ViewHistoryId = viewHistory.Id.Value,
            ViewedBy = new UserDTO
            {
                Id = viewHistory.ViewedBy.Id.Value,
                Username = viewHistory.ViewedBy.Login.Value,
                UserProfileUrl = viewHistory.ViewedBy.PhotoUrl?.Value!
            },
            ViewedVideo = new VideoDTO
            {
                Id = viewHistory.ViewedVideo.Id.Value,
                VideoTitle = viewHistory.ViewedVideo.VideoTitle?.Value,
                VideoUrl = viewHistory.ViewedVideo.VideoUrl?.Value,
                VideoPublisherId = viewHistory.ViewedVideo.Publisher.Id.Value,
                VideoThumbnail = viewHistory.ViewedVideo.VideoThumbnail?.Value,
                VideoDate = viewHistory.ViewedVideo.VideoPublishedDate.Value,
                VideoDescription = viewHistory.ViewedVideo.VideoDescription?.Value,
                VideoViews = viewHistory.ViewedVideo.VideoView.Value
            },
            ViewedAt = viewHistory.ViewedAt.Value,
            Owner = new UserDTO
            {
                Id = viewHistory.ViewedVideo.Publisher.Id.Value,
                Username = viewHistory.ViewedVideo.Publisher.Login.Value,
                UserProfileUrl = viewHistory.ViewedVideo.Publisher.PhotoUrl?.Value!
            }
        };
    }
}