using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Dislike;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.Dislike.HasDisliked;

public class Request
{
    public Guid Id { get; set; }
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
        Get("/video/{id}/dislike");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var didUserLike = await _dbContext.Set<Dislike<Video>>()
            .AnyAsync(x => (Guid)x.DislikedBy.Id == userId && (Guid)x.Parent.Id == req.Id,
                cancellationToken: ct);
        if (didUserLike)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}