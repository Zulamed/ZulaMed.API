using System.Text.Json;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Mux.Csharp.Sdk.Api;
using Mux.Csharp.Sdk.Model;
using OneOf;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.LiveStream;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using LiveStream = ZulaMed.API.Domain.LiveStream.LiveStream;

namespace ZulaMed.API.Endpoints.MuxLiveStream.StartLiveStream;

public class CreateLiveStreamCommand : Mediator.ICommand<OneOf<Success<LiveStream>, UserNotFound, Error<string>>>
{
    public required Guid UserId { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public string? VideoThumbnail { get; init; }
}

public class
    CreateLiveStreamCommandHandler : Mediator.ICommandHandler<CreateLiveStreamCommand,
        OneOf<Success<LiveStream>, UserNotFound, Error<string>>>
{
    private readonly LiveStreamsApi _liveStreamsApi;
    private readonly ZulaMedDbContext _dbContext;
    private readonly ILogger<CreateLiveStreamCommandHandler> _logger;

    public CreateLiveStreamCommandHandler(LiveStreamsApi liveStreamsApi, ZulaMedDbContext dbContext,
        ILogger<CreateLiveStreamCommandHandler> logger)
    {
        _liveStreamsApi = liveStreamsApi;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async ValueTask<OneOf<Success<LiveStream>, UserNotFound, Error<string>>> Handle(
        CreateLiveStreamCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _dbContext.Set<User>()
                .SingleOrDefaultAsync(x => command.UserId == (Guid)x.Id, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("User not found: {UserId}", command.UserId);
                return new UserNotFound();
            }


            var streamGuid = Guid.NewGuid();

            var videoGuid = Guid.NewGuid();


            var video = new Video
            {
                Id = VideoId.From(videoGuid),
                VideoPublishedDate = VideoPublishedDate.From(DateTime.UtcNow),
                VideoStatus = VideoStatus.WaitingForUpload,
                Publisher = user,
                VideoTitle = VideoTitle.From(command.Name),
                VideoDescription = command.Description is null ? null : VideoDescription.From(command.Description),
                VideoThumbnail = command.VideoThumbnail is null ? null : VideoThumbnail.From(command.VideoThumbnail)
            };


            var playbackPolicies = new List<PlaybackPolicy> { PlaybackPolicy.Public };

            var response = await _liveStreamsApi.CreateLiveStreamAsync(new CreateLiveStreamRequest()
            {
                PlaybackPolicy = playbackPolicies,
                Passthrough = JsonSerializer.Serialize(new
                {
                    StreamId = streamGuid
                }),
                NewAssetSettings = new CreateAssetRequest
                {
                    PlaybackPolicy = playbackPolicies
                }
            }, cancellationToken);

            var addedEntity = await _dbContext.Set<LiveStream>().AddAsync(new LiveStream
            {
                Id = LiveStreamId.From(streamGuid),
                RelatedVideo = video,
                Status = LiveStreamStatus.Idle,
                PlaybackId = response.Data.PlaybackIds.First().Id,
                StreamKey = response.Data.StreamKey
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Live stream created: {StreamId}", addedEntity.Entity.Id.Value.ToString("N"));

            return new Success<LiveStream>(addedEntity.Entity);
        }
        catch (DbUpdateException e)
        {
            _logger.LogWarning("An error occured while saving to database: {Message}", e.Message);
            return new Error<string>(e.Message);
        }
        catch (ValueObjectValidationException e)
        {
            _logger.LogWarning("Invalid data provided: {Message}", e.Message);
            return new Error<string>(e.Message);
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
        Post("/stream");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken cancellationToken)
    {
        var command = new CreateLiveStreamCommand
        {
            UserId = req.UserId,
            Name = req.Name,
            Description = req.Description,
            VideoThumbnail = req.VideoThumbnail
        };

        var result = await _mediator.Send(command, cancellationToken);

        await result.Match(
            s => SendAsync(new Response
            {
                PlaybackId = s.Value.PlaybackId,
                StreamId = s.Value.Id.Value,
                StreamKey = s.Value.StreamKey
            }, 201, cancellationToken),
            notFound => SendNotFoundAsync(cancellationToken),
            error =>
            {
                AddError(error.Value);
                return SendErrorsAsync(cancellation: cancellationToken);
            }
        );
    }
}