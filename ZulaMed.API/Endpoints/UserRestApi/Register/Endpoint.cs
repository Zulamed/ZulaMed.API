using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.Accounts.PersonalAccount;
using ZulaMed.API.Domain.Accounts.UniversityAccount;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;
using ZulaMed.API.Endpoints.UserRestApi.Register.Personal;
using ZulaMed.API.Endpoints.UserRestApi.Register.University;
using VoException = Vogen.ValueObjectValidationException;

namespace ZulaMed.API.Endpoints.UserRestApi.Register;

public class CreateVideoCommandHandler : Mediator.ICommandHandler<CreateUserCommand, Result<User, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;

    public CreateVideoCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth)
    {
        _dbContext = dbContext;
        _auth = auth;
    }

    public async ValueTask<Result<User, Exception>> Handle(CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<User>();
        try
        {
            var entity = await dbSet.AddAsync(new User
            {
                Id = (UserId)Guid.NewGuid(),
                Login = (UserLogin)command.Login,
                Email = (UserEmail)command.Email,
                Name = (UserName)command.Name,
                Surname = (UserSurname)command.Surname,
                Country = (UserCountry)command.Country,
                City = (UserCity)command.City,
                HistoryPaused = (HistoryPaused)false
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await AddUserToFirebase(command.Email, command.Password, entity.Entity.Id.Value, cancellationToken);
            return entity.Entity;
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

    private async Task AddUserToFirebase(string email, string password,Guid userId, CancellationToken token)
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

public class Endpoint : Endpoint<Request, UserDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        switch (request.AccountType)
        {
            case AccountType.Personal:
                var personalResult = await _mediator.Send(new CreatePersonalAccountCommand
                {
                    Email = request.Email,
                    Login = request.Login,
                    Password = request.Password,
                    Name = request.Name,
                    Surname = request.Surname,
                    Country = request.Country,
                    City = request.City,
                    AccountGender = request.AccountGender.Value,
                    AccountTitle = request.AccountTitle,
                    AccountCareerStage = request.AccountCareerStage,
                    AccountProfessionalActivity = request.AccountProfessionalActivity,
                    AccountSpecialty = request.AccountSpecialty,
                    AccountDepartment = request.AccountDepartment,
                    AccountBirthDate = request.AccountBirthDate.Value,
                    AccountInstitute = request.AccountInstitute,
                    AccountRole = request.AccountRole,
                    PlacesOfWork = request.PlacesOfWork
                }, ct);
                if (personalResult.TryPickT0(out var personalValue, out var error1))
                {
                    await SendOkAsync(new UserDTO
                    {
                        Id = personalValue.UserId.Value,
                        Email = personalValue.User.Email.Value,
                        Login = personalValue.User.Login.Value,
                        Name = personalValue.User.Name.Value,
                        Surname = personalValue.User.Surname.Value,
                        Country = personalValue.User.Country.Value,
                        City = personalValue.User.City.Value,
                        HistoryPaused = personalValue.User.HistoryPaused.Value
                    }, ct);
                    return;
                }
                AddError(error1.Value.Message);
                break;
            case AccountType.Hospital:
                var hospitalResult = await _mediator.Send(new CreateHospitalAccountCommand
                {
                    Email = request.Email,
                    Login = request.Login,
                    Password = request.Password,
                    Name = request.Name,
                    Surname = request.Surname,
                    Country = request.Country,
                    City = request.City,
                    AccountHospital = request.AccountHospital,
                    AccountAddress = request.AccountAddress,
                    AccountPostCode = request.AccountPostCode,
                    AccountPhone = request.AccountPhone,
                }, ct);
                if (hospitalResult.TryPickT0(out var hospitalAccount, out var error2))
                {
                    await SendOkAsync(new UserDTO
                    {
                        Id = hospitalAccount.UserId.Value,
                        Email = hospitalAccount.User.Email.Value,
                        Login = hospitalAccount.User.Login.Value,
                        Name = hospitalAccount.User.Name.Value,
                        Surname = hospitalAccount.User.Surname.Value,
                        Country = hospitalAccount.User.Country.Value,
                        City = hospitalAccount.User.City.Value,
                        HistoryPaused = hospitalAccount.User.HistoryPaused.Value
                    }, ct);
                    return; 
                }
                AddError(error2.Value.Message);
                break;
            case AccountType.University:
                var universityResult = await _mediator.Send(new CreateUniversityAccountCommand()
                {
                    Email = request.Email,
                    Login = request.Login,
                    Password = request.Password,
                    Name = request.Name,
                    Surname = request.Surname,
                    Country = request.Country,
                    City = request.City,
                    AccountUniversity = request.AccountUniversity,
                    AccountAddress = request.AccountAddress,
                    AccountPostCode = request.AccountPostCode,
                    AccountPhone = request.AccountPhone,
                }, ct);
                if (universityResult.TryPickT0(out var universityAccount, out var error))
                {
                    await SendOkAsync(new UserDTO
                    {
                        Id = universityAccount.UserId.Value,
                        Email = universityAccount.User.Email.Value,
                        Login = universityAccount.User.Login.Value,
                        Name = universityAccount.User.Name.Value,
                        Surname = universityAccount.User.Surname.Value,
                        Country = universityAccount.User.Country.Value,
                        City = universityAccount.User.City.Value,
                        HistoryPaused = universityAccount.User.HistoryPaused.Value
                    }, ct);
                    return; 
                }
                AddError(error.Value.Message);
                break;
        }
        await SendErrorsAsync(cancellation: ct);
    }
}