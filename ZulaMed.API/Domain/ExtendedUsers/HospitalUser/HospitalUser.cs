namespace ZulaMed.API.Domain.ExtendedUsers.HospitalUser;

public class HospitalUser
{
    public required User.User User { get; init; }
    public required UserHospital UserHospital { get; init; }
    public required UserAddress UserAddress { get; init; }
    public required UserPostCode UserPostCode { get; init; }
    public required UserPhone UserPhone { get; init; }
}