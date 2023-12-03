using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetByUser;

public class GetByUserQuery : IQuery<ValueTuple<Video[], int>>
{
    public required Guid Id { get; init; }

    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetByTitleQueryHandler : IQueryHandler<GetByUserQuery, ValueTuple<Video[], int>>
{
    private readonly ZulaMedDbContext _context;

    public GetByTitleQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<(Video[], int)> Handle(GetByUserQuery query, CancellationToken cancellationToken)
    {
        var count = await _context.Set<Video>()
            .Where(x => (Guid)x.Publisher.Id == query.Id && 
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .CountAsync(cancellationToken: cancellationToken);
        
        var videos = await _context.Set<Video>()
            .Include(v => v.Publisher)
            .Where(x => (Guid)x.Publisher.Id == query.Id && 
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken);
        return (videos, count);
    }
}