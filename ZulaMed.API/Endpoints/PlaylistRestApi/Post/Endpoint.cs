using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Post;

public class CreatePlaylistCommandHandler : Mediator.ICommandHandler<CreatePlaylistCommand, Result<Playlist, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;

    public CreatePlaylistCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Result<Playlist, Exception>> Handle(CreatePlaylistCommand command,
        CancellationToken cancellationToken)
    {
        var owner = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(x => (Guid)x.Id == command.OwnerId, cancellationToken);
        if (owner is null)
        {
            return new Error<Exception>(new Exception("Owner by provided id was not found"));
        }

        try
        {
            var dbSet = _dbContext.Set<Playlist>();
            var entity = await dbSet.AddAsync(new Playlist
            {
                Id = (PlaylistId)Guid.NewGuid(),
                Owner = owner,
                PlaylistName = (PlaylistName)command.PlaylistName,
                PlaylistDescription = (PlaylistDescription)command.PlaylistDescription,
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Entity;
        }
        catch (Exception e)
        {
            return new Error<Exception>(e);
        }
    }
}

public class Endpoint : Endpoint<Request, PlaylistDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/playlist");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new CreatePlaylistCommand
        {
            OwnerId = userId,
            PlaylistName = req.PlaylistName,
            PlaylistDescription = req.PlaylistDescription
        }, ct);
        if (result.TryPickT0(out var value, out var error))
        {
            await SendOkAsync(value.MapToResponse(), ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}