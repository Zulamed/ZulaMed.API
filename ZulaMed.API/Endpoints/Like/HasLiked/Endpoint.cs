using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Domain.Like;

namespace ZulaMed.API.Endpoints.Like.HasLiked;

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
        Get("/video/{id}/like");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var didUserLike = await _dbContext.Set<Like<Video>>()
            .AnyAsync(x => (Guid)x.LikedBy.Id == userId && (Guid)x.Parent.Id == req.Id,
                cancellationToken: ct);
        if (didUserLike)
        {
            await SendOkAsync(ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
}