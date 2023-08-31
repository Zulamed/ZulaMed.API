using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetSubscriptions;

public class Request
{
    public Guid UserId { get; init; }
}

public class Subscription
{
    public required UserDTO User { get; init; }
    public required int NumberOfSubscribers { get; init; }
}

public class Response
{
    public required Subscription[] Subscriptions { get; init; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ZulaMedDbContext _dbContext;

    public Endpoint(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/user/{UserId}/subscriptions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var subscriptions = await _dbContext
            .Set<User>()
            .Where(x => (Guid)x.Id == req.UserId)
            .SelectMany(x => x.Subscriptions)
            .Include(x => x.Group)
            .Select(x => new
            {
                Subscription = x,
                SubscriptionCount = x.Subscribers.Count
            })
            .ToListAsync(ct);
        if (subscriptions.Count == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = new Response
        {
            Subscriptions = subscriptions.Select(x => new Subscription()
            {
                NumberOfSubscribers = x.SubscriptionCount,
                User = x.Subscription.ToResponse()
            }).ToArray()
        };

        await SendAsync(response, cancellation: ct);
    }
}