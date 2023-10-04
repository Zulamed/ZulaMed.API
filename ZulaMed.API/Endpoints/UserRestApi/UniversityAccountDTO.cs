namespace ZulaMed.API.Endpoints.UserRestApi;

public class UniversityAccountDTO
{
    public required Guid UserId { get; init; }
    public required string AccountUniversity { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
}