using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.Comments.SendACommentToVideo;

public static class Mapper
{
   public static Response ToDomain(this Comment comment)
   {
       return new Response
       {
           Id = (Guid)comment.Id,
           Content = (string)comment.Content,
           SentBy = (Guid)comment.SentBy.Id,
           SentAt = (DateTime)comment.SentAt,
           RelatedVideo = (Guid)comment.RelatedVideo.Id
       };

   } 
}