using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.GetHistoryByUser;

public class
    GetViewHistoriesByUserQueryHandler : IQueryHandler<GetViewHistoriesByUserQuery, Response>
{
    private readonly ZulaMedDbContext _context;

    public GetViewHistoriesByUserQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Response> Handle(GetViewHistoriesByUserQuery query,
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

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/viewHistory/{ownerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetViewHistoriesByUserQuery
        {
            OwnerId = req.OwnerId,
            PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
        }, ct);

        await SendAsync(response, cancellation: ct);
    }
}