using FastEndpoints;
using FluentValidation;
using ZulaMed.API.Domain.Accounts.PersonalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Personal;

public class Request
{
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required bool AccountGender { get; init; }
    public required string AccountTitle { get; init; }
    public required string AccountCareerStage { get; init; }
    public required string AccountProfessionalActivity { get; init; }
    public required string AccountSpecialty { get; init; }
    public required string AccountDepartment { get; init; }
    public required DateTime AccountBirthDate { get; init; }
    public required string AccountInstitute { get; init; }
    public required string AccountRole { get; init; }
    public required List<string> PlacesOfWork { get; init; } = new();
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.AccountGender).NotEmpty();
        RuleFor(x => x.AccountTitle).NotEmpty();
        RuleFor(x => x.AccountCareerStage).NotEmpty();
        RuleFor(x => x.AccountProfessionalActivity).NotEmpty();
        RuleFor(x => x.AccountSpecialty).NotEmpty();
        RuleFor(x => x.AccountDepartment).NotEmpty();
        RuleFor(x => x.AccountBirthDate).NotEmpty();
        RuleFor(x => x.AccountInstitute).NotEmpty();
        RuleFor(x => x.AccountRole).NotEmpty();
    }
}

public class CreatePersonalAccountCommand : Mediator.ICommand<Result<PersonalAccount, Exception>>
{
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
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