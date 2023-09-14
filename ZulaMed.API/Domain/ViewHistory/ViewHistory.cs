namespace ZulaMed.API.Domain.ViewHistory;

public class ViewHistory
{
    public required Id Id { get; init; } = null!;
    public required ViewedAt ViewedAt { get; init; } 
    public required Video.Video ViewedVideo { get; init; }
    public required User.User ViewedBy { get; init; }
}