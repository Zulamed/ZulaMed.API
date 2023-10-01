using FastEndpoints;
using FluentValidation;
using ZulaMed.API.Domain.Accounts.HospitalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;

public class Request
{
    public required Guid UserId { get; init; }
    public required string AccountHospital { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
}
public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AccountHospital).NotEmpty();
        RuleFor(x => x.AccountAddress).NotEmpty();
        RuleFor(x => x.AccountPostCode).NotEmpty();
        RuleFor(x => x.AccountPhone).NotEmpty();
    }
}
public class CreateHospitalAccountCommand : Mediator.ICommand<Result<HospitalAccount, Exception>>
{
    public required Guid UserId { get; init; }
    public required string AccountHospital { get; init; }
    public required string AccountAddress { get; init; }
    public required string AccountPostCode { get; init; }
    public required string AccountPhone { get; init; }
}