using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;


public class GetAllVideosQueryHandler : IQueryHandler<GetAllVideosQuery, Video[]>
{
    private readonly ZulaMedDbContext _context;

    public GetAllVideosQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    public async ValueTask<Video[]> Handle(GetAllVideosQuery query, CancellationToken cancellationToken)
    {
        var videos = await _context.Set<Video>().ToArrayAsync(cancellationToken: cancellationToken);
        return videos;
    }
}

public class Endpoint : EndpointWithoutRequest<Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/video");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var videos = await _mediator.Send(new GetAllVideosQuery(), ct);

        await SendAsync(new Response
        {
            Videos = videos.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}