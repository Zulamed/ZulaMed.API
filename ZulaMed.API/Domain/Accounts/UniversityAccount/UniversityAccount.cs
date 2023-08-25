namespace ZulaMed.API.Domain.Accounts.UniversityAccount;

public class UniversityAccount
{
    public required User.User User { get; init; }
    public required AccountAddress AccountAddress { get; init; }
    public required AccountPostCode AccountPostCode { get; init; }
    public required AccountPhone AccountPhone { get; init; }
} 