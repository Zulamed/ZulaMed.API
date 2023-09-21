using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistoryByUser;

public class Request
{
    public required Guid OwnerId { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class DeleteHistoryByUserCommand : Mediator.ICommand<bool>
{
    public required Guid OwnerId { get; init; }
}