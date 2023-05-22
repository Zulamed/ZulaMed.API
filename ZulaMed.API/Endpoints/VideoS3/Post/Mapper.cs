namespace ZulaMed.API.Endpoints.VideoS3.Post;

public static class Mapper
{
   public static Response MapToResponse(this UploadResponse uploadResponse)
   {
      return new Response
      {
         VideoUrl = uploadResponse.VideoUrl
      };
   }

   public static UploadVideoCommand MapToCommand(this Request request)
   {
      return new UploadVideoCommand()
      {
         Video = request.Video
      };
   }
}