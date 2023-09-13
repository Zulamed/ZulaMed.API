namespace ZulaMed.API.Domain.ViewHistory;

public class ViewHistory
{
    public required ViewHistoryId ViewHistoryId { get; init; }
    public required Video.Video ViewedVideo { get; init; }
    public required User.User ViewedBy { get; init; }
    public required ViewedAt ViewedAt { get; init; } 
}