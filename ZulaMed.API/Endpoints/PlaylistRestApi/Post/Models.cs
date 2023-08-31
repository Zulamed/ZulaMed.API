using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Post;

public class Request
{
    public required Guid OwnerId { get; init; }
    public required string PlaylistName { get; init; } 
    public required string PlaylistDescription { get; init; } 
}

public class Validator : Validator<Request>
{ 
    public Validator()
    {
        RuleFor(x => x.PlaylistName).NotEmpty();
        RuleFor(x => x.PlaylistDescription).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}

public class CreatePlaylistCommand : Mediator.ICommand<Result<Domain.Playlist.Playlist, Exception>>
{
    public required Guid OwnerId { get; init; }
    public required string PlaylistName { get; init; } 
    public required string PlaylistDescription { get; init; } 
}