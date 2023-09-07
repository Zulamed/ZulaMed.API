using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.DeleteVideo;

public class Request
{
    public Guid PlaylistId { get; init; }
    public required Guid VideoId { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PlaylistId).NotEmpty();
        RuleFor(x => x.VideoId).NotEmpty();
    }
}

public class DeleteVideoFromPlaylistCommand : Mediator.ICommand<Result<bool, Exception>>
{
    public required Guid PlaylistId { get; init; }
    public required Guid VideoId { get; init; }
}