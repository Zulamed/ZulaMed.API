using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Delete;

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

public class DeletePlaylistCommand : Mediator.ICommand<bool>
{
    public required Guid OwnerId { get; init; }
    public required Guid Id { get; init; }
}