using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Domain.Comments;

public class Comment
{
    public required Id Id { get; init; } = null!;
    
    public required CommentContent Content { get; init; }

    public required User.User SentBy { get; init; }

    public required CommentSentDate SentAt { get; init; }

    public required Video.Video RelatedVideo { get; init; }
}