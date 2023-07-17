using FastEndpoints;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.UserRestApi.Subscribe;

public class Request
{
    public required Guid SubscriberId { get; init; }
    public required Guid SubToUserId { get; init; }
}

public class SubscribeCommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public required Guid SubscriberId { get; init; }
    public required Guid SubToUserId { get; init; }
}
