using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.SpecialtyGroup;
using ZulaMed.API.Domain.User;
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
                Email = (UserEmail)command.Email,
                Group = (await _dbContext.Set<SpecialtyGroup>()
                    .FirstOrDefaultAsync(x => (int)x.Id == command.GroupId, cancellationToken))!,
                Name = (UserName)command.Name,
                Surname = (UserSurname)command.Surname,
                Country = (UserCountry)command.Country,
                City = (UserCity)command.City,
                University = (UserUniversity)command.University,
                WorkPlace = (UserWorkPlace)command.WorkPlace
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