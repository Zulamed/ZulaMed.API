using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.VideoRestApi.Dislike;

public class Request
{
    public Guid Id { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    } 
}