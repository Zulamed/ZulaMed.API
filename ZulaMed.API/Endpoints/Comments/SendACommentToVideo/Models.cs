namespace ZulaMed.API.Endpoints.Comments.SendACommentToVideo;

public class Request 
{
    public required Guid VideoId { get; init; }
    public required string Content { get; init; }
    public required Guid SentBy { get; init; }
}


public class Response
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required Guid SentBy { get; init; }
    public required DateTime SentAt { get; init; }
    public required Guid RelatedVideo { get; init; }
}