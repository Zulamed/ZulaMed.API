namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

public class PersonalAccount
{
    public required User.User User { get; init; }
    public required AccountGender AccountGender { get; init; }
    public required AccountTitle AccountTitle { get; init; }
    public required AccountCareerStage AccountCareerStage { get; init; }
    public required AccountProfessionalActivity AccountProfessionalActivity { get; init; }
    public required AccountSpecialty AccountSpecialty { get; init; }
    public required AccountDepartment AccountDepartment { get; init; }
    public required AccountBirthDate AccountBirthDate { get; init; }
    public required AccountInstitute AccountInstitute { get; init; }
    public required AccountRole AccountRole { get; init; }
    public required List<string> PlacesOfWork { get; init; } = new();
}