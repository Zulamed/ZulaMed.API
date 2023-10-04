namespace ZulaMed.API.Endpoints.UserRestApi;

public class PersonalAccountDTO
{
    public required Guid UserId { get; init; }
    public required bool AccountGender { get; init; }
    public required string AccountTitle { get; init; }
    public required string AccountCareerStage { get; init; }
    public required string AccountProfessionalActivity { get; init; }
    public required string AccountSpecialty { get; init; }
    public required string AccountDepartment { get; init; }
    public required DateOnly AccountBirthDate { get; init; }
    public required string AccountInstitute { get; init; }
    public required string AccountRole { get; init; }
    public required List<string> PlacesOfWork { get; init; } = new();
}