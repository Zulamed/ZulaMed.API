using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.UserRestApi.Unsubscribe;

public class Request
{
    public Guid UnsubFromUserId { get; init; }
}

public class UnsubscribeCommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public required Guid SubscriberId { get; init; }
    public required Guid UnsubFromUserId { get; init; }
}
