using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.Get.GetByUser;

public class GetByUserQuery : IQuery<Response>
{
    public required Guid OwnerId { get; init; }
    public required PaginationOptions PaginationOptions { get; init; }
}

public class
    GetByUserQueryHandler : IQueryHandler<GetByUserQuery, Response>
{
    private readonly ZulaMedDbContext _context;

    public GetByUserQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Response> Handle(GetByUserQuery query,
        CancellationToken cancellationToken)
    {
        var count = await _context.Set<Domain.ViewHistory.ViewHistory>()
            .Where(x => (Guid)x.ViewedBy.Id == query.OwnerId).CountAsync(cancellationToken);

        var viewHistories = await _context.Set<Domain.ViewHistory.ViewHistory>()
            .Where(x => (Guid)x.ViewedBy.Id == query.OwnerId)
            .Include(x => x.ViewedVideo)
            .ThenInclude(x => x.Publisher)
            .Include(x => x.ViewedBy)
            .Paginate(x => x.ViewedAt, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return new Response
        {
            ViewHistories = viewHistories.Select(x => x.ToResponse())
                .ToArray(),
            Total = count
        };
    }
}