using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetByTitle;

public class GetByTitleQueryHandler : IQueryHandler<GetByTitleQuery, Video[]>
{
    private readonly ZulaMedDbContext _context;

    public GetByTitleQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Video[]> Handle(GetByTitleQuery query, CancellationToken cancellationToken)
    {
        var videos = await _context.Set<Video>().Where(x => x.VideoTitle.Value.Contains(query.Title)).ToArrayAsync(cancellationToken: cancellationToken);
        return videos;
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
        Get("/video/{title}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var videos = _mediator.Send(new GetByTitleQuery
        {
            Title = req.Title
        }, ct);

        await SendAsync(new Response()
        {
            Videos = videos.Result.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}