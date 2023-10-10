using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.VideoRestApi;

public record VideoDTO
{
    public required Guid Id { get; init; }
    public string? VideoTitle { get; init; }
    public required Guid VideoPublisherId { get; init; }
    public string? VideoUrl { get; init; }
    public string? VideoThumbnail { get; init; }
    public string? VideoDescription { get; init; }
    
    public string? VideoTimelineThumbnail { get; init; }
    
    public required DateTime VideoPublishedDate { get; init; }
    
    public required long VideoViews { get; init; }
}