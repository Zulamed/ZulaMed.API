using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;
using ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetAll;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetByUser;

public class GetPlaylistsByUserQueryHandler : IQueryHandler<GetPlaylistsByUserQuery, Playlist[]>
{
    private readonly ZulaMedDbContext _context;

    public GetPlaylistsByUserQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Playlist[]> Handle(GetPlaylistsByUserQuery query, CancellationToken cancellationToken)
    {
        var playlists = await _context.Set<Playlist>()
            .Where(x => x.Owner.Id == query.OwnerId)
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
        Get("/playlist/{userid}");
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