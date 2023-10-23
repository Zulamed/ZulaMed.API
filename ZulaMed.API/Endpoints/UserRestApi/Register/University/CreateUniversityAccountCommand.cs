using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.UniversityAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.University;

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
public class CreateUniversityAccountCommandHandler : Mediator.ICommandHandler<CreateUniversityAccountCommand,
    Result<UniversityAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;

    public CreateUniversityAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth)
    {
        _dbContext = dbContext;
        _auth = auth;
    }

    public async ValueTask<Result<UniversityAccount, Exception>> Handle(CreateUniversityAccountCommand command,
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

            var account = new UniversityAccount
            {
                User = user,
                AccountAddress = (AccountAddress)command.AccountAddress,
                AccountPostCode = (AccountPostCode)command.AccountPostCode,
                AccountPhone = (AccountPhone)command.AccountPhone,
                AccountUniversity = (AccountUniversity)command.AccountUniversity 
            };
            var accountEntity = await _dbContext.Set<UniversityAccount>().AddAsync(account, cancellationToken);
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
