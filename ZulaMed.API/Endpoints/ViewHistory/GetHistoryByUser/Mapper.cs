namespace ZulaMed.API.Endpoints.ViewHistory.GetHistoryByUser;

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
                Login = viewHistory.ViewedBy.Login.Value
            },
            ViewedVideo = new VideoDTO
            {
                Id = viewHistory.ViewedVideo.Id.Value,
                VideoTitle = viewHistory.ViewedVideo.VideoTitle.Value,
                VideoPublisherId = viewHistory.ViewedVideo.Publisher.Id.Value,
                VideoThumbnail = viewHistory.ViewedVideo.VideoThumbnail.Value,
                VideoDescription = viewHistory.ViewedVideo.VideoDescription.Value,
                VideoViews = viewHistory.ViewedVideo.VideoView.Value
            },
            ViewedAt = viewHistory.ViewedAt.Value,
        };
    }
}