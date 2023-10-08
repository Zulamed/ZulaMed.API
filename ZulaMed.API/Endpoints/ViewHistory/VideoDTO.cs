namespace ZulaMed.API.Endpoints.ViewHistory;

public class VideoDTO
{
    public required Guid Id { get; init; }
    public string? VideoTitle { get; init; }
    public required Guid VideoPublisherId { get; init; }
    public string? VideoThumbnail { get; init; }
    public string? VideoDescription { get; init; }
    public required long VideoViews { get; init; }
}