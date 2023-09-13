namespace ZulaMed.API.Endpoints.ViewHistory.GetHistoryByUser;

public static class Mapper
{
    public static ViewHistoryDTO ToResponse(this Domain.ViewHistory.ViewHistory viewHistory)
    {
        return new ViewHistoryDTO
        {
            ViewHistoryId = viewHistory.ViewHistoryId.Value,
            ViewedBy = viewHistory.ViewedBy,
            ViewedVideo = viewHistory.ViewedVideo,
            ViewedAt = viewHistory.ViewedAt.Value,
        };
    }
}