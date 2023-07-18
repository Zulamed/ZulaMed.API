using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
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

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(SubscribeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var subscriber = await _dbContext.Set<User>().FirstOrDefaultAsync(x => (Guid)x.Id == command.SubscriberId, cancellationToken);
            var subToUser = await _dbContext.Set<User>().FirstOrDefaultAsync(x => (Guid)x.Id == command.SubToUserId, cancellationToken);
            if (subscriber is null || subToUser is null)
            {
                return new NotFound();
            }
            subscriber.Subscriptions.Add(subToUser);
            subToUser.Subscribers.Add(subscriber);
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
        Post("user/{subToUserId}/subscribe");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new SubscribeCommand
        {
            SubscriberId = req.SubscriberId,
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