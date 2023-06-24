using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public class Response
{
    public required VideoDTO? Video { get; set; }
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

public class GetVideoByIdQuery : IQuery<Response>
{
    public required Guid Id { get; init; }
}