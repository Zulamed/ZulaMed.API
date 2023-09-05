using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetSubscriptions;

public class Request
{
    public Guid UserId { get; init; }
}

public class Response
{
    public required UserDTO[] Subscriptions { get; init; }
}

public class Endpoint : Endpoint<Request>
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
        var user = await _dbContext
            .Set<User>()
            .Include(x => x.Subscriptions)
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => (Guid)x.Id == req.UserId, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct); 
            return;
        }
        await SendAsync(new Response
        {
            Subscriptions = user.Subscriptions.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}