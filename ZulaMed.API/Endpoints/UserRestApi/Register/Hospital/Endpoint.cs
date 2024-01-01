using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;

public class CreateHospitalAccountCommandHandler : Mediator.ICommandHandler<CreateHospitalAccountCommand,
    Result<HospitalAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly FirebaseAuth _auth;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public CreateHospitalAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth, IOptions<S3BucketOptions> s3options)
    {
        _dbContext = dbContext;
        _auth = auth;
        _s3Options = s3options;
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
                HistoryPaused = (HistoryPaused)false,
                PhotoUrl = (PhotoUrl)$"{_s3Options.Value.BaseUrl}/users/images/hospital.jpg",
                RegistrationTime = (RegistrationTime)DateTime.UtcNow
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

public class Endpoint : Endpoint<Request, HospitalAccountDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/hospital");
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