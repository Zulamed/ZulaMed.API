using Google.Apis.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get.GetByTitle;


public class GetByTitleQuery : IQuery<Video[]>
{
    public required string Title { get; init; }
    
    public required PaginationOptions PaginationOptions { get; init; }
}


public class GetByTitleQueryHandler : IQueryHandler<GetByTitleQuery, Video[]>
{
    private readonly ZulaMedDbContext _context;

    public GetByTitleQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Video[]> Handle(GetByTitleQuery query, CancellationToken cancellationToken)
    {
        var videos = await _context.Set<Video>()
            .Include(x => x.Publisher)
            .Where(x => EF.Functions.ILike((string)x.VideoTitle, $"%{query.Title}%"))
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return videos;
    }
}