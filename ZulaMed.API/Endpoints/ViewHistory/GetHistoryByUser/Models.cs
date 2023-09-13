using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.ViewHistory.GetHistoryByUser;

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

public class Response
{
    public required ViewHistoryDTO[] ViewHistories { get; init; }
}

public class GetViewHistoriesByUserQuery : IQuery<Domain.ViewHistory.ViewHistory[]>
{
    public required Guid OwnerId { get; init; }
}