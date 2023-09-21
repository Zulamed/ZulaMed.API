using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.Get.GetByTitle;

public class GetByTitleQuery : IQuery<Response>
{
    public required Guid OwnerId { get; init; }
    public required string Title { get; init; }
    public required PaginationOptions PaginationOptions { get; init; }
}

public class GetByTitleQueryHandler : IQueryHandler<GetByTitleQuery, Response>
{
    private readonly ZulaMedDbContext _context;

    public GetByTitleQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Response> Handle(GetByTitleQuery query,
        CancellationToken cancellationToken)
    {
        var count = await _context.Set<Domain.ViewHistory.ViewHistory>()
            .Where(x => (Guid)x.ViewedBy.Id == query.OwnerId).CountAsync(cancellationToken);
        var histories = await _context.Set<Domain.ViewHistory.ViewHistory>()
            .Where(x => EF.Functions.ILike((string)x.ViewedVideo.VideoTitle, $"%{query.Title}%") &&
                        (Guid)x.ViewedBy.Id == query.OwnerId)
            .Include(x => x.ViewedVideo)
            .ThenInclude(x => x.Publisher)
            .Include(x => x.ViewedBy)
            .Paginate(x => x.ViewedVideo.VideoPublishedDate, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return new Response
        {
            ViewHistories = histories.Select(x => x.ToResponse())
                .ToArray(),
            Total = count
        };
    }
}