using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Subscriptions;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Subscribe;

public class
    SubscribeCommandHandler : Mediator.ICommandHandler<SubscribeCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public SubscribeCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(SubscribeCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var subscriber = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.SubscriberId, cancellationToken);
            var subToUser = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.SubToUserId, cancellationToken);
            if (subscriber is null || subToUser is null)
            {
                return new NotFound();
            }

            _dbContext.Set<Subscription>().Add(new Subscription()
            {
                Subscriber = subscriber,
                SubscribedTo = subToUser
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            FormattableString sql =
                $"""
                 UPDATE "User"
                 SET "SubscriberCount" = "SubscriberCount" + 1
                 WHERE "Id" = {command.SubToUserId}
                 """;
            await _dbContext.Database.ExecuteSqlAsync(sql, cancellationToken: cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            return new Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new Error<string>(e.Message);
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
        Post("user/{subToUserId}/subscribe");
        Description(c => c.Produces(200), clearDefaults: true);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        if (userId == req.SubToUserId)
        {
            AddError("You can't subscribe to yourself!");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var result = await _mediator.Send(new SubscribeCommand
        {
            SubscriberId = userId,
            SubToUserId = req.SubToUserId
        }, ct);
        await result.Match(
            a => SendOkAsync(ct),
            e => SendAsync(new
            {
                StatusCode = 500,
                Message = e.Value
            }, 500, ct),
            n => SendNotFoundAsync(ct)
        );
    }
}