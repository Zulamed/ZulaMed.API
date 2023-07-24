namespace ZulaMed.API.Domain.Comments;

public class Comment
{
    public required CommentId Id { get; init; }

    public required CommentContent Content { get; init; }

    public required User.User SentBy { get; init; }

    public required CommentSentDate SentAt { get; init; }

    public required Video.Video RelatedVideo { get; init; }
}