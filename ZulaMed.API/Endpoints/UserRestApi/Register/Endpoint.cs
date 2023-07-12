using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.SpecialtyGroup;
using ZulaMed.API.Domain.User;
using VoException = Vogen.ValueObjectValidationException;

namespace ZulaMed.API.Endpoints.UserRestApi.Register;

public class CreateVideoCommandHandler : Mediator.ICommandHandler<CreateUserCommand, Result<User, VoException>>
{
    private readonly ZulaMedDbContext _dbContext;

    public CreateVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result<User, ValueObjectValidationException>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<User>();
        try
        {
            var entity = await dbSet.AddAsync(new User
            {
                Id = (UserId)Guid.NewGuid(),
                Email = (UserEmail)command.Email,
                Group = (await _dbContext.Set<SpecialtyGroup>().FirstOrDefaultAsync(x => (int)x.Id == command.GroupId, cancellationToken))!,
                Name = (UserName)command.Name,
                Surname = (UserSurname)command.Surname,
                Country = (UserCountry)command.Country,
                City = (UserCity)command.City,
                University = (UserUniversity)command.University,
                WorkPlace = (UserWorkPlace)command.WorkPlace
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Entity;
        }
        catch (VoException e)
        {
            return new Error<VoException>(e);
        }
        
    }
}

public class Endpoint : Endpoint<Request, UserDTO>
{
    private readonly IMediator _mediator;
    private readonly FirebaseAuth _auth;

    public Endpoint(IMediator mediator, FirebaseAuth auth)
    {
        _mediator = mediator;
        _auth = auth;
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
            await _auth.CreateUserAsync(new UserRecordArgs()
            {
                Email = request.Email,
                Password = request.Password
            }, ct);
            await SendOkAsync(value.MapToResponse(), ct);
            return;
        }
        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}