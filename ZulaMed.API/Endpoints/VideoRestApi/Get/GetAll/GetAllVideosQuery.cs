using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;

public class GetAllVideosQuery : IQuery<Video[]>
{
    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetAllVideosQueryHandler : IQueryHandler<GetAllVideosQuery, Video[]>
{
    private readonly ZulaMedDbContext _context;

    public GetAllVideosQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    public async ValueTask<Video[]> Handle(GetAllVideosQuery query, CancellationToken cancellationToken)
    {
        var videos = await _context.Set<Video>()
            .Include(x => x.Publisher)
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return videos;
    }
}