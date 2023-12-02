using FastEndpoints;
using FluentValidation;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public class Request 
{
    public Guid Id { get; init; }
    
    public required string VideoTitle { get; init; }
    
    public string? VideoDescription { get; init; }
}


public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.VideoTitle).NotEmpty();
    }
}

public class UpdateVideoCommand : Mediator.ICommand<Result<bool, ValueObjectValidationException>>
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string VideoTitle { get; init; }
    
    public string? VideoDescription { get; init; }
}