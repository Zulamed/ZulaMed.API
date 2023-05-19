using Amazon.S3.Model;

namespace ZulaMed.API.Endpoints.Video.Post;

public class Request
{
   public required IFormFile Video { get; set; }
}

public class Response
{
   public required string VideoUrl { get; init; }
}

public class UploadResponse
{
    public required string VideoUrl { get; set; }
    
    public required PutObjectResponse PutResponse { get; set; }
}

public class UploadVideoCommand : Mediator.ICommand<UploadResponse>
{
    public required IFormFile Video { get; set; }
}