using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, Response>
{
    private readonly ZulaMedDbContext _context;

    public GetVideoByIdQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Response> Handle(GetVideoByIdQuery query, CancellationToken cancellationToken)
    {
        var video = await _context.Set<Video>().FirstAsync(x => x.Id == query.Id, cancellationToken);
        return video.ToResponse();
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
        Get("/video/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetVideoByIdQuery
        {
            Id = req.Id
        }, ct);

        await SendAsync(response, cancellation: ct);
    }
}