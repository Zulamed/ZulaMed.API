using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.ViewHistory.GetComments;

public static class Mapper
{
    public static CommentDTO ToDTO(this Comment comment)
    {
        return new CommentDTO
        {
            Id = (Guid)comment.Id,
            Content = (string)comment.Content,
            SentAt = (DateTime)comment.SentAt,
        };
    } 
}