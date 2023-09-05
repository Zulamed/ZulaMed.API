using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.Comments.GetCommentsForAVideo;

public static class Mapper
{
   public static CommentDTO ToDTO(this Comment comment)
   {
      return new()
      {
         Id = (Guid)comment.Id,
         Content = (string)comment.Content,
         SentBy = new UserMinimalDTO
         {
            Id = (Guid)comment.SentBy.Id,
            Username = (string)comment.SentBy.Login,
            ProfilePictureUrl = comment.SentBy.PhotoUrl?.Value
         },
         SentAt = (DateTime)comment.SentAt,
         RelatedVideo = (Guid)comment.RelatedVideo.Id
      };
   } 
}