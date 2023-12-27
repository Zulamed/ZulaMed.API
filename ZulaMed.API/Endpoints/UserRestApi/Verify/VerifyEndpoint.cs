using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Verify;

public class VerifyEndpoint : EndpointWithoutRequest
{
    private readonly ZulaMedDbContext _dbContext;

    public VerifyEndpoint(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/user/verify");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var rows = await _dbContext.Set<User>()
            .Where(x => (Guid)x.Id == userId && (bool)x.IsVerified)
            .ExecuteUpdateAsync(calls => 
                    calls.SetProperty(u => u.IsVerified, IsVerified.From(true)), cancellationToken: ct);
        if (rows <= 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(ct);
    }
}