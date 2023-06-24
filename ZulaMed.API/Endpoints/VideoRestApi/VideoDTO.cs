namespace ZulaMed.API.Endpoints.VideoRestApi;

public record VideoDTO
{
    public required Guid Id { get; init; }
    public required string VideoTitle { get; init; }
    public required string VideoUrl { get; init; }
    public required string VideoThumbnail { get; init; }
    public required string VideoDescription { get; init; }
    public required DateTime VideoPublishedDate { get; init; }
}