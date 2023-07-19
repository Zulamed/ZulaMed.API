namespace ZulaMed.API.Endpoints.Comments;

public class UserMinimalDTO
{
    public required Guid Id { get; init; }
    
    public required string Username { get; init; }
    
    public string? ProfilePictureUrl { get; init; }
}

public class CommentDTO
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required UserMinimalDTO SentBy { get; init; } 
    public required DateTime SentAt { get; init; }
    public required Guid RelatedVideo { get; init; }
}