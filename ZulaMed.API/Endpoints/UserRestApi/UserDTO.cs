using ZulaMed.API.Domain.SpecialtyGroup;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class UserDTO
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Group { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string University { get; init; }
    public required string WorkPlace { get; init; }
    public required bool HistoryPaused { get; init; }
    
    public string? ProfilePictureUrl { get; init; }
}