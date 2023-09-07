using FastEndpoints;
using FluentValidation;
using Vogen;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.AddVideos;

public class Request
{
    public Guid PlaylistId { get; init; }
    public required List<Guid> VideosIds { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PlaylistId).NotEmpty();
        RuleFor(x => x.VideosIds).NotEmpty();
    }
}

public class AddVideosToPlaylistCommand : Mediator.ICommand<Result<bool, Exception>>
{
    public required Guid PlaylistId { get; init; }
    public required List<Guid> VideosIds { get; init; }
}