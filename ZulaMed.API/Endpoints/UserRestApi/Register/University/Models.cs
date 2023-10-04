using FastEndpoints;
using FluentValidation;
using ZulaMed.API.Domain.Accounts.UniversityAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.University;

public class Request
{
    public required string AccountUniversity { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.AccountUniversity).NotEmpty();
        RuleFor(x => x.AccountAddress).NotEmpty();
        RuleFor(x => x.AccountPostCode).NotEmpty();
        RuleFor(x => x.AccountPhone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
    }
}

public class CreateUniversityAccountCommand : Mediator.ICommand<Result<UniversityAccount, Exception>>
{
    public required string AccountUniversity { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
}