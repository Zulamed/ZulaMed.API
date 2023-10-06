using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Domain.Accounts.HospitalAccount;

public class HospitalAccount
{
    public UserId UserId { get; init; }
    public required User.User User { get; init; }
    public required AccountHospital AccountHospital { get; init; }
    public required AccountAddress AccountAddress { get; init; }
    public required AccountPostCode AccountPostCode { get; init; }
    public required AccountPhone AccountPhone { get; init; }
}