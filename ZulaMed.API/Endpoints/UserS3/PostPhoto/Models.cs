using FastEndpoints;

namespace ZulaMed.API.Endpoints.UserS3.PostPhoto;

public class Request
{
    public required IFormFile Photo { get; init; }
    public required Guid UserId { get; init; }
}

public class RequestValidator : Validator<Request>
{
    
}