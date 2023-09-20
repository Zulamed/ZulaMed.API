using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.ViewHistory.ToggleHistory;

public class Request
{
    public required Guid Id { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class ToggleHistoryCommand : Mediator.ICommand<bool>
{
    public required Guid Id { get; init; }
}