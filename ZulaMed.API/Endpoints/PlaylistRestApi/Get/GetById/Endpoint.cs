using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetById;

public class GetPlaylistByIdQueryHandler : IQueryHandler<GetPlaylistByIdQuery, Response>
{
    private readonly ZulaMedDbContext _dbContext;

    public GetPlaylistByIdQueryHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Response> Handle(GetPlaylistByIdQuery query, CancellationToken cancellationToken)
    {
        var playlist = await _dbContext.Set<Playlist>()
            .FirstOrDefaultAsync(x => (Guid)x.Id == query.PlaylistId, cancellationToken);
        return new Response
        {
            Playlist = playlist?.ToResponse()
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
        Get("/playlist/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetPlaylistByIdQuery
        {
            PlaylistId = req.PlaylistId
        }, ct);
        if (response.Playlist is null)
        {
            await SendNotFoundAsync(ct);
        }

        await SendAsync(response, cancellation: ct);
    }
}