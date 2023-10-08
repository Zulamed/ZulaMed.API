using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;

public class GetAllVideosQuery : IQuery<(Video[], int)>
{
    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetAllVideosQueryHandler : IQueryHandler<GetAllVideosQuery, (Video[], int)>
{
    private readonly ZulaMedDbContext _context;

    public GetAllVideosQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    public async ValueTask<(Video[], int)> Handle(GetAllVideosQuery query, CancellationToken cancellationToken)
    {
        var count = await _context.Set<Video>()
            .CountAsync(cancellationToken: cancellationToken);
        
        var videos = await _context.Set<Video>()
            .Include(x => x.Publisher)
            .Where(x => x.VideoStatus == VideoStatus.Ready && !x.VideoTitle!.Equals((object?)null))
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return (videos, count);
    }
}