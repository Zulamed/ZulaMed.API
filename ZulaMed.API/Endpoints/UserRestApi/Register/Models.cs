using FastEndpoints;
using FluentValidation;
using Vogen;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register;

public class Request
{
    public required string Email { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Name  { get; init; }
    public required string Surname  { get; init; }
    public required string Country  { get; init; }
    public required string City  { get; init; }
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
}