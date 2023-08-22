namespace ZulaMed.API.Domain.ExtendedUsers.UniversityUser;

public class UniversityUser
{
    public required User.User User { get; init; }
    public required UserAddress UserAddress { get; init; }
    public required UserPostCode UserPostCode { get; init; }
    public required UserPhone UserPhone { get; init; }
} 