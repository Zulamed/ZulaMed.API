using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Put;

public class UpdateUserCommandHandler : Mediator.ICommandHandler<UpdateUserCommand, Result<bool, ValueObjectValidationException>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UpdateUserCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async ValueTask<Result<bool, ValueObjectValidationException>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Set<User>()
                .Where(x => (Guid)x.Id == command.Id)
                .ExecuteUpdateAsync(calls => calls
                    .SetProperty(x => x.Name, (UserName)command.Name)
                    .SetProperty(x => x.Surname, (UserSurname)command.Surname)
                    .SetProperty(x => x.Country, (UserCountry)command.Country)
                    .SetProperty(x => x.City, (UserCity)command.City)
                    .SetProperty(x => x.Email, (UserEmail)command.Email),
                    cancellationToken);
            return rows > 0;

        }
        catch (ValueObjectValidationException e)
        {
            return new Error<ValueObjectValidationException>(e);
        }
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/user/{id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new UpdateUserCommand
        {
            Id = userId,
            Email = req.Email,
            Name = req.Name,
            Surname = req.Surname,
            Country = req.Country,
            City = req.City,
        }, ct);
        if (result.TryPickT0(out var isUpdated, out var error))
        {
            if (!isUpdated)
            {
                await SendNotFoundAsync(ct);
                return;
            }
            await SendOkAsync(ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}