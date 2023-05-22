using Amazon.S3.Model;
using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.Video.Post;

public class Request
{
   public required IFormFile Video { get; set; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Video)
            .NotNull()
            .WithMessage("Video is required")
            .Must(x => x.ContentType.StartsWith("video/"))
            .WithMessage("Sent file must be a video file");
    } 
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