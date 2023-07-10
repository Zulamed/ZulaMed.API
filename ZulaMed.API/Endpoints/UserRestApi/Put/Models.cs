using FastEndpoints;
using FluentValidation;
using Vogen;

namespace ZulaMed.API.Endpoints.UserRestApi.Put;

public class Request
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required int GroupId  { get; init; }
    public required string Name  { get; init; }
    public required string Surname  { get; init; }
    public required string Country  { get; init; }
    public required string City  { get; init; }
    public required string University  { get; init; }
    public required string WorkPlace  { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.University).NotEmpty();
        RuleFor(x => x.WorkPlace).NotEmpty();
    }
}

public class UpdateUserCommand : Mediator.ICommand<Result<bool, ValueObjectValidationException>>
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required int GroupId  { get; init; }
    public required string Name  { get; init; }
    public required string Surname  { get; init; }
    public required string Country  { get; init; }
    public required string City  { get; init; }
    public required string University  { get; init; }
    public required string WorkPlace  { get; init; }
}