using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;

public class GetAllVideosQuery : IQuery<Video[]>
{
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
        var videos = await _context.Set<Video>().Include(x => x.Publisher)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return videos;
    }
}