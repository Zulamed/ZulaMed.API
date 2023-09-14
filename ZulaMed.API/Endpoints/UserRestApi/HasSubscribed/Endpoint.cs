using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.HasSubscribed;

public class Request
{
    public Guid UserId { get; set; }
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
        Get("/user/{userId}/subscribe");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        // var isSubscribedTo = await _dbContext.Set<User>()
        //     .AnyAsync(u => (Guid)u.Id == userId && u.Subscriptions.Any(x => (Guid)x.Id == req.UserId)
        //         ,cancellationToken: ct);


        // fuck ef core
          var isSubscribedTo = await _dbContext.Database
              .SqlQuery<bool>(
                  $"""
                   SELECT EXISTS 
                      (SELECT 1 FROM "Subscription"
                          WHERE "SubscriberId" = {userId} AND "SubscribedToId"= {req.UserId}) as "Value"
                   """)
              .SingleOrDefaultAsync(cancellationToken: ct);
        if (isSubscribedTo)
        {
            await SendOkAsync(ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
}