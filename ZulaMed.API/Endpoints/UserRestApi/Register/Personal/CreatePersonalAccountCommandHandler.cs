using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts.PersonalAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Personal;

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
public class CreatePersonalAccountCommandHandler : Mediator.ICommandHandler<CreatePersonalAccountCommand,
    Result<PersonalAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;

    public CreatePersonalAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth)
    {
        _dbContext = dbContext;
        _auth = auth;
    }

    public async ValueTask<Result<PersonalAccount, Exception>> Handle(CreatePersonalAccountCommand command,
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

            var account = new PersonalAccount
            {
                User = user,
                AccountGender = (AccountGender)(command.AccountGender ? Gender.Male : Gender.Female),
                AccountTitle = (AccountTitle)command.AccountTitle,
                AccountCareerStage = (AccountCareerStage)command.AccountCareerStage,
                AccountProfessionalActivity = (AccountProfessionalActivity)command.AccountProfessionalActivity,
                AccountSpecialty = (AccountSpecialty)command.AccountSpecialty,
                AccountDepartment = (AccountDepartment)command.AccountDepartment,
                AccountBirthDate = (AccountBirthDate)command.AccountBirthDate,
                AccountInstitute = (AccountInstitute)command.AccountInstitute,
                AccountRole = (AccountRole)command.AccountRole,
                PlacesOfWork = command.PlacesOfWork,
            };
            var accountEntity = await _dbContext.Set<PersonalAccount>().AddAsync(account, cancellationToken);
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