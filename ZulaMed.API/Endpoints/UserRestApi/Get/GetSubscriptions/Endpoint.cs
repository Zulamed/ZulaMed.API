using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Subscriptions;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetSubscriptions;

public class Request
{
    public Guid UserId { get; init; }

    [QueryParam] public int Page { get; init; } = 1;

    [QueryParam] public int PageSize { get; init; } = 10;
}

public class SubscriptionDTO
{
    public required UserDTO User { get; init; }
    public required int NumberOfSubscribers { get; init; }
}

public class Response
{
    public required SubscriptionDTO[] Subscriptions { get; init; }
    
    public required int Total { get; init; }
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
        var count = _dbContext.Set<Subscription>().Count(x => (Guid)x.Subscriber.Id == req.UserId);
        var subscriptions = await _dbContext
            .Set<Subscription>()
            .Where(x => (Guid)x.Subscriber.Id == req.UserId)
            .Include(x => x.SubscribedTo)
            .Paginate(x => x.SubscribedTo.Login, new PaginationOptions(req.Page, req.PageSize))
            .Select(x => new
            {
                Subscription = x.SubscribedTo,
                SubscriptionCount = (int)x.SubscribedTo.SubscriberCount
            })
            .ToListAsync(ct);
        if (subscriptions.Count == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var response = new Response
        {
            Subscriptions = subscriptions.Select(x => new SubscriptionDTO()
            {
                NumberOfSubscribers = x.SubscriptionCount,
                User = x.Subscription.ToResponse()
            }).ToArray(),
            Total = count
        };
        
        await SendAsync(response, cancellation: ct);
    }
}