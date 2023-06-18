using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.UserS3.GetPhoto;

public class Response
{
    public required string PhotoUrl { get; init; }
}

public class Request
{
    public required Guid Id { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class GetUserPhotoQuery : IQuery<Response>
{
    public required Guid UserId { get; init; }
}

