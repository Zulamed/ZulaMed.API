using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class GetByUserQuery : IQuery<ValueTuple<Video[], int>>
{
    public required Guid UserId { get; init; }

    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetByUserQueryHandler : IQueryHandler<GetByUserQuery, ValueTuple<Video[], int>>
{
    private readonly ZulaMedDbContext _context;

    public GetByUserQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<(Video[], int)> Handle(GetByUserQuery query, CancellationToken cancellationToken)
    {
        var count = await _context.Set<Video>()
            .Where(x => x.Publisher.Id == (UserId)query.UserId &&
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .CountAsync(cancellationToken: cancellationToken);

        var videos = await _context.Set<Video>()
            .Include(x => x.Publisher)
            .Where(x => x.Publisher.Id == (UserId)query.UserId &&
                        x.VideoStatus == VideoStatus.Ready
                        && !x.VideoTitle!.Equals((object?)null))
            .Paginate(x => x.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return (videos, count);
    }
}