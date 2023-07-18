using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
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

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(UnsubscribeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var subscriber = await _dbContext.Set<User>().FirstOrDefaultAsync(x => (Guid)x.Id == command.SubscriberId, cancellationToken);
            var unSubToUser = await _dbContext.Set<User>().Include(x => x.Subscribers).FirstOrDefaultAsync(x => (Guid)x.Id == command.UnsubFromUserId, cancellationToken);
            if (subscriber is null || unSubToUser is null)
            {
                return new NotFound();
            }
            // subscriber.Subscriptions.Remove(subToUser);
            unSubToUser.Subscribers.Remove(subscriber);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new Success();
        }
        catch (Exception e)
        {
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new UnsubscribeCommand
        {
            SubscriberId = req.SubscriberId,
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