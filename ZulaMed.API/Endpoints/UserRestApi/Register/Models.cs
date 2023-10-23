using FastEndpoints;
using FluentValidation;
using Vogen;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register;

public enum AccountType
{
    Personal,
    Hospital,
    University
}

public class Request
{
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name  { get; init; }
    public required string Surname  { get; init; }
    public required string Country  { get; init; }
    public required string City  { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
    
    public required AccountType AccountType { get; init; }
    
    public string? AccountUniversity { get; init; } // university account
    
    public string? AccountHospital { get; init; } // hospital account
    
    public bool? AccountGender { get; init; } // personal account below
    public string? AccountTitle { get; init; }
    public string? AccountCareerStage { get; init; }
    public string? AccountProfessionalActivity { get; init; }
    public string? AccountSpecialty { get; init; }
    public string? AccountDepartment { get; init; }
    public DateOnly? AccountBirthDate { get; init; }
    public string? AccountInstitute { get; init; }
    public string? AccountRole { get; init; }
    public List<string> PlacesOfWork { get; init; } = new();
    
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
        RuleFor(x => x.AccountAddress).NotEmpty();
        RuleFor(x => x.AccountPostCode).NotEmpty();
        RuleFor(x => x.AccountPhone).NotEmpty();
        RuleFor(x => x.AccountType).NotEmpty();
    }
}

public class CreateUserCommand : Mediator.ICommand<Result<User, Exception>>
{
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Name  { get; init; }
    public required string Surname  { get; init; }
    public required string Country  { get; init; }
    public required string City  { get; init; }
    public required string Password { get; init; }
    
    public required AccountType AccountType { get; init; }
    
    public string? AccountUniversity { get; init; } // university account
    
    public string? AccountHospital { get; init; } // hospital account
    
    public bool? AccountGender { get; init; } // personal account below
    public string? AccountTitle { get; init; }
    public string? AccountCareerStage { get; init; }
    public string? AccountProfessionalActivity { get; init; }
    public string? AccountSpecialty { get; init; }
    public string? AccountDepartment { get; init; }
    public DateOnly? AccountBirthDate { get; init; }
    public string? AccountInstitute { get; init; }
    public string? AccountRole { get; init; }
    public List<string> PlacesOfWork { get; init; } = new();
}