using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistoryByUser;

public class Request
{
    [QueryParam]
    public Guid? OwnerId { get; init; }
}

public class DeleteHistoryByUserCommand : Mediator.ICommand<bool>
{
    public required Guid OwnerId { get; init; }
}