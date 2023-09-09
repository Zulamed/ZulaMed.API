using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.Comments.GetCommentsForAVideo;

public class GetCommentsForAVideoQueryHandler : IQueryHandler<GetCommentsForAVideoQuery, Response?>
{
    private readonly ZulaMedDbContext _dbContext;

    public GetCommentsForAVideoQueryHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Response?> Handle(GetCommentsForAVideoQuery query, CancellationToken cancellationToken)
    {
        var comments = await _dbContext
            .Set<Comment>()
            .Where(x => (Guid)x.RelatedVideo.Id == query.VideoId)
            .Include(x => x.SentBy)
            .Paginate(x => x.SentAt, query.PaginationOptions)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return new Response
        {
            Comments = comments.Select(x => x.ToDTO()).ToList()
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
        Get("/video/{videoId}/comment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetCommentsForAVideoQuery
        {
            VideoId = req.VideoId,
            PaginationOptions = new PaginationOptions(req.Page, req.PageSize),
        }, ct);
        if (response is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(response, cancellation: ct);
    }
}