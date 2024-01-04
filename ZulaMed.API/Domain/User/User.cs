using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.Accounts.PersonalAccount;
using ZulaMed.API.Domain.Accounts.UniversityAccount;
using ZulaMed.API.Domain.Subscriptions;

namespace ZulaMed.API.Domain.User;

public class User
{
    public required UserId Id { get; init; }
    public required UserLogin Login { get; init; }
    public required UserName Name { get; init; }
    public required UserSurname Surname { get; init; }
    public required UserEmail Email { get; init; }
    public required UserCountry Country { get; init; }
    public required UserCity City { get; init; }
    public PhotoUrl? PhotoUrl { get; set; }
    public BannerUrl? BannerUrl { get; set; }
    public Description? Description { get; set; }
    public HistoryPaused HistoryPaused { get; init; } 
    public SubscriberCount SubscriberCount { get; init; } = SubscriberCount.Zero;
    public IsVerified IsVerified { get; init; } = IsVerified.From(false);
    
    public required RegistrationTime RegistrationTime { get; init; }
    public List<Subscription> Subscriptions { get; init; } = new();
    public List<Subscription> Subscribers { get; init; } = new();
    public List<Video.Video> Videos { get; init; } = new();
    public HospitalAccount? HospitalAccount { get; init; }
    public UniversityAccount? UniversityAccount { get; init; }
    public PersonalAccount? PersonalAccount { get; init; }
}