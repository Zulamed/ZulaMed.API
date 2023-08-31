using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Delete;

public class DeletePlaylistCommandHandler : Mediator.ICommandHandler<DeletePlaylistCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public DeletePlaylistCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(DeletePlaylistCommand command, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Set<Playlist>().Where(x => command.Id == (Guid)x.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        return rows > 0;
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
        Delete("/playlist/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.ToCommand(), ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}