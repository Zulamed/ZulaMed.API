namespace ZulaMed.API.Domain.User;

public class User
{
    public required UserId Id { get; init; }
    
    public required UserEmail Email { get; init; }
    
    public required SpecialtyGroup.SpecialtyGroup Group { get; init; }
    
    public required UserName Name { get; init; }
    
    public required UserSurname Surname { get; init; }
    
    public required UserCountry Country { get; init; }
    
    public required UserCity City { get; init; }
    
    public required UserUniversity University { get; init; }
    
    public required UserWorkPlace WorkPlace { get; init; }
    public PhotoUrl PhotoUrl { get; set; }
    public List<User> Subscriptions { get; init; } = new();
    public List<User> Subscribers { get; init; } = new();
}