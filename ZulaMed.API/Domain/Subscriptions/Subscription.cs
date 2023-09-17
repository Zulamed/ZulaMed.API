using ZulaMed.API.Domain.Shared;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Domain.Subscriptions;

public class Subscription
{
    public Id SubscriberId { get; set; } = null!;
    public required User.User Subscriber { get; set; }

    public Id SubscribedToId { get; set; } = null!;
    public required User.User SubscribedTo { get; set; }
    
}