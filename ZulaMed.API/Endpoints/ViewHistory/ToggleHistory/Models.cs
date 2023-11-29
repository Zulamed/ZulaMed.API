using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.ViewHistory.ToggleHistory;

public class ToggleHistoryCommand : Mediator.ICommand<bool>
{
    public required Guid Id { get; init; }
}