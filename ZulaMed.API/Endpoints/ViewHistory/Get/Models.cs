using FastEndpoints;
using FluentValidation;
using Mediator;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.Get;

public class Request
{
    public required Guid OwnerId { get; init; }
    
    [QueryParam]
    public string? Title { get; init; }
    
    [QueryParam]
    public int Page { get; init; } = 1;
    
    [QueryParam]
    public int PageSize { get; init; } = 10;
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
    public required int Total { get; init; }
}

