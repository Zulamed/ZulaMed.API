
namespace ZulaMed.API.Endpoints.UserRestApi;

public class UserDTO
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required bool HistoryPaused { get; init; }
    
    public bool IsVerified { get; init; }
    
    public string? Description { get; init; }
    public string? ProfilePictureUrl { get; init; }
}