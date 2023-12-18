using Amazon.S3.Model;
using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.UserS3.PostPhoto;

public class Request
{
    public required IFormFile Photo { get; init; }
    public required Guid UserId { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Id is required");
        RuleFor(x => x.Photo)
            .NotNull()
            .WithMessage("Photo is required");
    }
}

public class Response
{
    public required string PhotoUrl { get; set; }
}

public class UploadResponse
{
    public required string PhotoUrl { get; set; }
    
    public required PutObjectResponse PutResponse { get; set; }
}

public class UploadPhotoCommand : Mediator.ICommand<UploadResponse?>
{
    public required IFormFile Photo { get; init; }
    public required Guid UserId { get; init; }
}
