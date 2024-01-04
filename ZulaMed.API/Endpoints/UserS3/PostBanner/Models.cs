using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.UserS3.PostBanner;

public class Request
{
    public required IFormFile Banner { get; init; }

    [FromClaim] public Guid UserId { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Banner)
            .NotEmpty()
            .Must(x => x.ContentType.Contains("image"))
            .WithMessage("Banner is required and must be an image")
            .Must(x => x.Length < 10_000_000)
            .WithMessage("Banner must be less than 10MB");
        
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public class Response
{
    public required string BannerUrl { get; set; }
}