namespace ZulaMed.API.Domain.Video;

public class Video
{
    public VideoId Id { get; init; }
    public VideoTitle VideoTitle { get; init; }
    public VideoUrl VideoUrl { get; init; }
    public VideoThumbnail VideoThumbnail { get; init; }
    public VideoDescription VideoDescription { get; init; }
    public VideoPublishedDate VideoPublishedDate { get; init; }
}