using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Subscriptions;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Unsubscribe;

public class
    UnsubscribeCommandHandler : Mediator.ICommandHandler<UnsubscribeCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UnsubscribeCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(UnsubscribeCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var subscriber = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.SubscriberId, cancellationToken);
            var unSubToUser = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.UnsubFromUserId, cancellationToken);
            if (subscriber is null || unSubToUser is null)
            {
                return new NotFound();
            }

            var subscription = new Subscription()
            {
                Subscriber = subscriber,
                SubscribedTo = unSubToUser
            };
            _dbContext.Set<Subscription>().Attach(subscription);
            _dbContext.Set<Subscription>().Remove(subscription);

            await _dbContext.SaveChangesAsync(cancellationToken);

            FormattableString sql =
                $"""
                 UPDATE "User"
                 SET "SubscriberCount" = "SubscriberCount" - 1
                 WHERE "Id" = {command.UnsubFromUserId}
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
        Post("user/{unsubFromUserId}/unsubscribe");
        Description(c => c.Produces(200), clearDefaults: true);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new UnsubscribeCommand
        {
            SubscriberId = userId,
            UnsubFromUserId = req.UnsubFromUserId
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