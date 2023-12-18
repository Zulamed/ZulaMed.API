using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts.PersonalAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Personal;

public class CreatePersonalAccountCommandHandler : Mediator.ICommandHandler<CreatePersonalAccountCommand,
    Result<PersonalAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public CreatePersonalAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth, IOptions<S3BucketOptions> s3Options)
    {
        _dbContext = dbContext;
        _auth = auth;
        _s3Options = s3Options;
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
                HistoryPaused = (HistoryPaused)false,
                PhotoUrl = (PhotoUrl)$"{_s3Options.Value.BaseUrl}/users/images/personal.jpg"
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

public class Endpoint : Endpoint<Request, PersonalAccountDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/personal");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var result = await _mediator.Send(request.MapToCommand(), ct);
        if (result.TryPickT0(out var value, out var error))
        {
            await SendOkAsync(value.MapToResponse(), ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}