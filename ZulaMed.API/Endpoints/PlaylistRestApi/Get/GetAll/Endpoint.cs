using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetAll;

public class GetAllPlaylistsQueryHandler : IQueryHandler<GetAllPlaylistsQuery, Playlist[]>
{
    private readonly ZulaMedDbContext _context;

    public GetAllPlaylistsQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Playlist[]> Handle(GetAllPlaylistsQuery query, CancellationToken cancellationToken)
    {
        var playlists = await _context.Set<Playlist>()
            .ToArrayAsync(cancellationToken: cancellationToken);
        return playlists;
    }
}
public class Endpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;
    
    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override void Configure()
    {
        Get("/playlist");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var playlists = await _mediator.Send(new GetAllPlaylistsQuery(), ct);

        await SendAsync(new Response
        {
            Playlists = playlists.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}