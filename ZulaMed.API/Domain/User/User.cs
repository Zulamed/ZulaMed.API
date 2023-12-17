using Vogen;
using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.Accounts.PersonalAccount;
using ZulaMed.API.Domain.Accounts.UniversityAccount;
using ZulaMed.API.Domain.Subscriptions;

namespace ZulaMed.API.Domain.User;



[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct Description 
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Description can't be null");
    }
}

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
    public Description? Description { get; set; }
    public HistoryPaused HistoryPaused { get; init; } 
    public SubscriberCount SubscriberCount { get; init; } = SubscriberCount.Zero;
    public List<Subscription> Subscriptions { get; init; } = new();
    public List<Subscription> Subscribers { get; init; } = new();
    public List<Video.Video> Videos { get; init; } = new();
    public HospitalAccount? HospitalAccount { get; init; }
    public UniversityAccount? UniversityAccount { get; init; }
    public PersonalAccount? PersonalAccount { get; init; }
}