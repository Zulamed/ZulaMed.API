using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Playlist;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.AddVideos;

public class AddVideosToPlaylistCommandHandler : Mediator.ICommandHandler<AddVideosToPlaylistCommand, Result<bool, Exception>>
{
    private readonly ZulaMedDbContext _dbContext;

    public AddVideosToPlaylistCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result<bool, Exception>> Handle(AddVideosToPlaylistCommand command, CancellationToken cancellationToken)
    {
        var playlist = await _dbContext.Set<Playlist>().FirstOrDefaultAsync(x => x.Id == command.PlaylistId, cancellationToken);
        if (playlist is null)
        {
            return new Error<Exception>(new Exception("Owner by provided id was not found"));
        }
        var videos = await _dbContext.Set<Video>().Where(x => command.VideosIds.Contains((Guid)x.Id)).ToArrayAsync(cancellationToken);
        try
        {
            playlist.Videos.AddRange(videos.ToList());
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
        Put("/playlist/{playlistId}/addVideos");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddVideosToPlaylistCommand
        {
            PlaylistId = req.PlaylistId,
            VideosIds = req.VideosIds
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