using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Subscriptions;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetUserLiked;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetByUserSubscriptions;

public class GetVideosByUserSubscriptionsQuery : IQuery<ValueTuple<Video[], int>>
{
    public required Guid UserId { get; init; }

    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetVideosByUserSubscriptionsQueryHandler : IQueryHandler<GetVideosByUserSubscriptionsQuery, ValueTuple<Video[], int>>
{
    private readonly ZulaMedDbContext _context;

    public GetVideosByUserSubscriptionsQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
 
    public async ValueTask<(Video[], int)> Handle(GetVideosByUserSubscriptionsQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => (Guid)x.Id == query.UserId, cancellationToken);
        if (user is null)
        {
            return (Array.Empty<Video>(), 0);
        }
        var count = await _context.Set<Video>()
            .Where(x => x.Publisher.Subscribers.Any(y => (Guid)y.SubscriberId == (Guid)user.Id) && 
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .CountAsync(cancellationToken: cancellationToken);
        
        var videos = await _context.Set<Video>()
            .Include(v => v.Publisher)
            .Where(x => x.Publisher.Subscribers.Any(y => (Guid)y.SubscriberId == (Guid)user.Id) && 
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken);
        
        return (videos, count);
    }
}