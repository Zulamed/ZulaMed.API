using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.Comments.GetCommentsForAVideo;

public class GetCommentsForAVideoQueryHandler : IQueryHandler<GetCommentsForAVideoQuery, Response>
{
    private readonly ZulaMedDbContext _dbContext;

    public GetCommentsForAVideoQueryHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Response> Handle(GetCommentsForAVideoQuery query, CancellationToken cancellationToken)
    {
        var video = await _dbContext.Set<Video>()
            .FirstOrDefaultAsync(x => x.Id == query.VideoId, cancellationToken: cancellationToken);
        return new Response
        {
            Comments = video?.Comments
        };
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
        Get("/video/{videoId}/comment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetCommentsForAVideoQuery()
        {
            VideoId = req.VideoId,
        }, ct);
        if (response.Comments is null)
        {
            await SendNotFoundAsync(ct);
        }
        await SendAsync(response, cancellation: ct);
    }
}