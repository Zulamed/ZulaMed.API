namespace ZulaMed.API.Endpoints.ViewHistory;

public class VideoDTO
{
    public required Guid Id { get; init; }
    public required string VideoTitle { get; init; }
    public required Guid VideoPublisherId { get; init; }
    public required string VideoThumbnail { get; init; }
    public required string VideoDescription { get; init; }
    public required long VideoViews { get; init; }
}