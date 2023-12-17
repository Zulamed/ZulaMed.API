using FastEndpoints;
using FluentValidation;
using Vogen;

namespace ZulaMed.API.Endpoints.UserRestApi.Put;

public class Request
{
    public string Description { get; init; } = null!;
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class UpdateUserCommand : Mediator.ICommand<Result<bool, ValueObjectValidationException>>
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }
}