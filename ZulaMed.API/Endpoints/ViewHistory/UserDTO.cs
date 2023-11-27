namespace ZulaMed.API.Endpoints.ViewHistory;

public class UserDTO
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string UserProfileUrl { get; init; }
}