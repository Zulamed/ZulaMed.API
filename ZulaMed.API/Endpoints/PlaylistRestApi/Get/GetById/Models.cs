using FastEndpoints;
using FluentValidation;
using Mediator;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetById;

public class Response
{
    public required PlaylistDTO? Playlist { get; init; }
}

public class Request
{
    public required Guid PlaylistId { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.PlaylistId)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class GetPlaylistByIdQuery : IQuery<Response>
{
    public required Guid PlaylistId { get; init; }
}