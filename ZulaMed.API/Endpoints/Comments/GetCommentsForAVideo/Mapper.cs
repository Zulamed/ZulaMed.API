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
            Username = (string)comment.SentBy.Name,
            ProfilePictureUrl = null // needs to bce changed to this -> ProfilePictureUrl = comment.SentBy.ProfilePictureUrl
         },
         SentAt = (DateTime)comment.SentAt,
         RelatedVideo = (Guid)comment.RelatedVideo.Id
      };
   } 
}