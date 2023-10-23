using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;

public class CreateHospitalAccountCommand : Mediator.ICommand<Result<HospitalAccount, Exception>>
{
    public required string AccountHospital { get; init; }
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
public class CreateHospitalAccountCommandHandler : Mediator.ICommandHandler<CreateHospitalAccountCommand,
    Result<HospitalAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;

    public CreateHospitalAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth)
    {
        _dbContext = dbContext;
        _auth = auth;
    }

    public async ValueTask<Result<HospitalAccount, Exception>> Handle(CreateHospitalAccountCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = new User
            {
                Id = (UserId)Guid.NewGuid(),
                Login = (UserLogin)command.Login,
                Email = (UserEmail)command.Email,
                Name = (UserName)command.Name,
                Surname = (UserSurname)command.Surname,
                Country = (UserCountry)command.Country,
                City = (UserCity)command.City,
                HistoryPaused = (HistoryPaused)false
            };
            var userEntity = await _dbContext.Set<User>().AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await AddUserToFirebase(command.Email, command.Password, userEntity.Entity.Id.Value, cancellationToken);

            var account = new HospitalAccount
            {
                User = user,
                AccountHospital = (AccountHospital)command.AccountHospital,
                AccountAddress = (AccountAddress)command.AccountAddress,
                AccountPostCode = (AccountPostCode)command.AccountPostCode,
                AccountPhone = (AccountPhone)command.AccountPhone
            };
            var accountEntity = await _dbContext.Set<HospitalAccount>().AddAsync(account, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return accountEntity.Entity;
        }
        catch (DbUpdateException e)
        {
            // needs better error message
            return new Error<Exception>(e.InnerException!);
        }
        catch (Exception e)
        {
            return new Error<Exception>(e);
        }
    }

    private async Task AddUserToFirebase(string email, string password, Guid userId, CancellationToken token)
    {
        var user = await _auth.CreateUserAsync(new UserRecordArgs()
        {
            Email = email,
            Password = password
        }, token);
        await _auth.SetCustomUserClaimsAsync(user.Uid, new Dictionary<string, object>()
        {
            ["IsAdmin"] = false,
            ["UserId"] = userId
        }, token);
    }
}
