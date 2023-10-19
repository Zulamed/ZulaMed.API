using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Put;

public class UpdatePlaylistCommandHandler : Mediator.ICommandHandler<UpdatePlaylistCommand, Result<bool, ValueObjectValidationException>>
{
    private readonly ZulaMedDbContext _dbContext;

    public UpdatePlaylistCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result<bool, ValueObjectValidationException>> Handle(UpdatePlaylistCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Set<Playlist>()
                .Where(x => (Guid)x.Id == command.PlaylistId && command.OwnerId == (Guid)x.Owner.Id)
                .ExecuteUpdateAsync(calls => calls
                    .SetProperty(x => x.PlaylistName, (PlaylistName)command.PlaylistName)
                    .SetProperty(x => x.PlaylistDescription, (PlaylistDescription)command.PlaylistDescription),
                    cancellationToken: cancellationToken);
            return rows > 0;
        }
        catch (ValueObjectValidationException e)
        {
            return new Error<ValueObjectValidationException>(e);
        }
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
        Put("/playlist/{id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new UpdatePlaylistCommand
        {
            PlaylistId = req.PlaylistId,
            OwnerId = userId,
            PlaylistName = req.PlaylistName,
            PlaylistDescription = req.PlaylistDescription
        }, ct);
        if (result.TryPickT0(out var isUpdated, out var error))
        {
            if (!isUpdated)
            {
                await SendNotFoundAsync(ct);
                return;
            }
            await SendOkAsync(ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}