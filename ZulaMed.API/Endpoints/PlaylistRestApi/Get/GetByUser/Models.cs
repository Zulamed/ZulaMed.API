using FastEndpoints;
using FluentValidation;
using Mediator;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetByUser;

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
    public required PlaylistDTO[] Playlists { get; init; }
}

public class GetPlaylistsByUserQuery : IQuery<Playlist[]>
{
    public required Guid OwnerId { get; init; }
}