using FastEndpoints;
using FluentValidation;
using Mediator;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.Get;

public class Request
{
    [QueryParam]
    public Guid? OwnerId { get; init; }
    
    [QueryParam]
    public string? Title { get; init; }
    
    [QueryParam]
    public int Page { get; init; } = 1;
    
    [QueryParam]
    public int PageSize { get; init; } = 10;
}

public class Response
{
    public required ViewHistoryDTO[] ViewHistories { get; init; }
    public required int Total { get; init; }
}

