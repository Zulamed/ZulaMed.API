using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.GetComments;

public class GetCommentsByUserQueryHandler : IQueryHandler<GetCommentsByUserQuery, Response?>
{
    private readonly ZulaMedDbContext _dbContext;

    public GetCommentsByUserQueryHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Response?> Handle(GetCommentsByUserQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await _dbContext
            .Set<Comment>()
            .Where(x => (Guid)x.SentBy.Id == query.OwnerId)
            .CountAsync(cancellationToken: cancellationToken);
        
        var comments = await _dbContext
            .Set<Comment>()
            .Where(x => (Guid)x.SentBy.Id == query.OwnerId)
            .Include(x => x.SentBy)
            .Paginate(x => x.SentAt, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return new Response
        {
            Comments = comments.Select(x => x.ToDTO()).ToList(),
            Total = totalCount
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
        Get("/viewHistory/{ownerId}/comment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetCommentsByUserQuery
        {
            PaginationOptions = new PaginationOptions(req.Page,
                req.PageSize),
            OwnerId = req.OwnerId,
        }, ct);
        if (response is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(response, cancellation: ct);
    }
}