using ZulaMed.API.Domain.Shared;
using ZulaMed.API.Domain.Subscriptions;

namespace ZulaMed.API.Domain.User;

public class User
{
    public Id Id { get; init; } = null!;
    
    public required UserLogin Login { get; init; }
    public required UserEmail Email { get; init; }
    
    public required SpecialtyGroup.SpecialtyGroup Group { get; init; }
    
    public required UserName Name { get; init; }
    
    public required UserSurname Surname { get; init; }
    
    public required UserCountry Country { get; init; }
    
    public required UserCity City { get; init; }
    
    public required UserUniversity University { get; init; }
    
    public required UserWorkPlace WorkPlace { get; init; }
    public PhotoUrl? PhotoUrl { get; set; } 
    
    public SubscriberCount SubscriberCount { get; init; } = SubscriberCount.Zero;
    
    public List<Subscription> Subscriptions { get; init; } = new();
    public List<Subscription> Subscribers { get; init; } = new();
    
    public List<Video.Video> Videos { get; init; } = new();
}