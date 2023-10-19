using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;
using ZulaMed.API.Domain.User;

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
        var rows = await _dbContext.Set<Playlist>().Where(x => command.Id == (Guid)x.Id && command.OwnerId == (Guid)x.Owner.Id)
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
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new DeletePlaylistCommand
        {
            OwnerId = userId,
            Id = req.Id
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}