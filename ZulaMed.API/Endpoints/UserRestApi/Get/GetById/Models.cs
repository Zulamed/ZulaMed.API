using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetById;

public class Response
{
    public required UserDTO? User { get; init; }
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

public class GetUserByIdQuery : IQuery<Response>
{
    public required Guid Id { get; init; }
}