using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetUserDetailed;

public class VideoMinimalDTO
{
    public required Guid Id { get; init; }
    
    public required string Title { get; init; }
    
    public required long Views { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required string Description { get; init; }
    
    public required string ThumbnailUrl { get; init; }
}


public class Request 
{
    public required Guid UserId { get; init; }
}

public class Response
{
    public required UserDTO User { get; init; }
    
    public required VideoMinimalDTO[] Videos { get; init; }
    
    public int NumberOfFollowers { get; init; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ZulaMedDbContext _dbContext;
    private readonly IOptions<S3BucketOptions> _options;

    public Endpoint(ZulaMedDbContext dbContext, IOptions<S3BucketOptions> s3Configuration)
    {
        _dbContext = dbContext;
        _options = s3Configuration;
    }

    public override void Configure()
    {
        Get("user/{UserId}/detailed");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await _dbContext
            .Set<User>()
            .Include(x => x.Videos.OrderByDescending(z => z.VideoPublishedDate))
            .FirstOrDefaultAsync(x => (Guid)x.Id == req.UserId, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct); 
            return;
        }

        var numberOfFollowers = await _dbContext
            .Entry(user)
            .Collection(x => x.Subscribers)
            .Query()
            .CountAsync(cancellationToken: ct);

        // var videos = await _dbContext.Entry(user)
        //     .Collection(x => x.Videos)
        //     .Query()
        //     .OrderByDescending(x => x.VideoPublishedDate)
        //     .ToArrayAsync(cancellationToken: ct);
        
        await SendAsync(new Response
        {
            NumberOfFollowers = numberOfFollowers,
            User = user.ToResponse(),
            Videos = user.Videos.Select(x => new VideoMinimalDTO
            {
                Id = x.Id.Value,
                Title = x.VideoTitle.Value,
                Views = x.VideoView.Value,
                CreatedAt = x.VideoPublishedDate.Value,
                ThumbnailUrl = $"{_options.Value.BaseUrl}{x.VideoThumbnail.Value}",
                Description = x.VideoDescription.Value
            }).ToArray()
        }, cancellation: ct);
    }
}