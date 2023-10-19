using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistory;

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

public class DeleteHistoryCommand : Mediator.ICommand<bool>
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
}