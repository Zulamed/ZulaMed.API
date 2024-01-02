using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Refit;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Verify;


public record VerifyEmailRequest([AliasAs("oobCode")]string OobCode);
public interface IFirebaseEmailVerifier
{
    [Post("/accounts:update")]
    Task<HttpResponseMessage> VerifyPasswordAsync([Body] VerifyEmailRequest request, [Query] string key);
}

public record Request(string OobCode);


public class VerifyEndpoint : Endpoint<Request>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly IFirebaseEmailVerifier _firebaseEmailVerifier;
    private readonly IConfiguration _configuration;

    public VerifyEndpoint(ZulaMedDbContext dbContext, IFirebaseEmailVerifier firebaseEmailVerifier, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _firebaseEmailVerifier = firebaseEmailVerifier;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Post("/user/verify");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var response = await _firebaseEmailVerifier.VerifyPasswordAsync(new VerifyEmailRequest(req.OobCode), _configuration["Firebase:ApiKey"]!);
        if (!response.IsSuccessStatusCode)
        {
            AddError("Invalid verification code");
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        var rows = await _dbContext.Set<User>()
            .Where(x => (Guid)x.Id == userId)
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