namespace ZulaMed.API.Endpoints.UserRestApi;

public class HospitalAccountDTO
{
    public required Guid UserId { get; init; }
    public required string AccountHospital { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
}
