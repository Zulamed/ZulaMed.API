using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Accounts.HospitalAccount;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;

public class CreateHospitalAccountCommandHandler : Mediator.ICommandHandler<CreateHospitalAccountCommand, Result<HospitalAccount, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;

    public CreateHospitalAccountCommandHandler(ZulaMedDbContext dbContext, FirebaseAuth auth)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<HospitalAccount, Exception>> Handle(CreateHospitalAccountCommand command,
        CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<HospitalAccount>();
        var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            
        }
        try
        {
            // var entity = await dbSet.AddAsync(new HospitalAccount
            // {
            //     User = null,
            //     AccountHospital = null,
            //     AccountAddress = null,
            //     AccountPostCode = null,
            //     AccountPhone = null
            // }, cancellationToken);
            // await _dbContext.SaveChangesAsync(cancellationToken);
            // return entity.Entity;
            return null;
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
        Post("/hospitalAccount");
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