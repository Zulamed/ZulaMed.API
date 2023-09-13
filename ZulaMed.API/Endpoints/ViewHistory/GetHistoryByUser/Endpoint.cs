using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;

namespace ZulaMed.API.Endpoints.ViewHistory.GetHistoryByUser;

public class GetViewHistoriesByUserQueryHandler : IQueryHandler<GetViewHistoriesByUserQuery, Domain.ViewHistory.ViewHistory[]>
{
    private readonly ZulaMedDbContext _context;

    public GetViewHistoriesByUserQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Domain.ViewHistory.ViewHistory[]> Handle(GetViewHistoriesByUserQuery query, CancellationToken cancellationToken)
    {
        var viewHistories = await _context.Set<Domain.ViewHistory.ViewHistory>()
            .Where(x => (Guid)x.ViewedBy.Id == query.OwnerId)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return viewHistories;
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/viewHistory/user/{ownerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var viewHistories = await _mediator.Send(new GetViewHistoriesByUserQuery { OwnerId = req.OwnerId }, ct);

        await SendAsync(new Response
        {
            ViewHistories = viewHistories.Select(x => x.ToResponse())
                .ToArray(),
        }, cancellation: ct);
    }
}