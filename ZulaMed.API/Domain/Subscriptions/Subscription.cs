using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Domain.Subscriptions;

public class Subscription
{
    public UserId SubscriberId { get; set; }
    public required User.User Subscriber { get; set; }
    
    public UserId SubscribedToId { get; set; }
    public required User.User SubscribedTo { get; set; }
    
}