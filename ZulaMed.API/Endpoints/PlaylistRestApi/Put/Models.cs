using FastEndpoints;
using FluentValidation;
using Vogen;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Put;

public class Request
{
    public required Guid PlaylistId { get; init; }
    public required string PlaylistName { get; init; } 
    public required string PlaylistDescription { get; init; } 
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PlaylistId).NotEmpty();
        RuleFor(x => x.PlaylistName).NotEmpty();
        RuleFor(x => x.PlaylistDescription).NotEmpty();
    }
}

public class UpdatePlaylistCommand : Mediator.ICommand<Result<bool, ValueObjectValidationException>>
{
    public required Guid PlaylistId { get; init; }
    public required Guid OwnerId { get; init; }
    public required string PlaylistName { get; init; } 
    public required string PlaylistDescription { get; init; } 
}