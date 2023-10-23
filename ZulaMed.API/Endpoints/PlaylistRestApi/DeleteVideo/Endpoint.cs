using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.DeleteVideo;

public class AddVideosToPlaylistCommandHandler : Mediator.ICommandHandler<DeleteVideoFromPlaylistCommand, Result<bool, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;

    public AddVideosToPlaylistCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result<bool, Exception>> Handle(DeleteVideoFromPlaylistCommand command, CancellationToken cancellationToken)
    {
        var playlist = await _dbContext
            .Set<Playlist>()
            .Include(x => x.Videos)
            .FirstOrDefaultAsync(x => (Guid)x.Id == command.PlaylistId, cancellationToken);
        if (playlist is null)
        {
            return new Error<Exception>(new Exception("Owner by provided id was not found"));
        }
        var video = playlist.Videos.FirstOrDefault(x => command.VideoId == (Guid)x.Id);
        if (video is null)
        {
            return new Error<Exception>(new Exception("Video by provided id was not found"));
        }
        if ((Guid)playlist.Owner.Id != command.OwnerId)
        {
            return new Error<Exception>(new Exception("User is not an owner of the playlist"));
        }
        try
        {
            playlist.Videos.Remove(video);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            return new Error<Exception>(e);
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
        Delete("/playlist/{playlistId}/video");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new DeleteVideoFromPlaylistCommand
        {
            PlaylistId = req.PlaylistId,
            VideoId = req.VideoId,
            OwnerId = userId
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