using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Mux.Csharp.Sdk.Api;
using Mux.Csharp.Sdk.Model;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using LiveStream = ZulaMed.API.Domain.LiveStream.LiveStream;

namespace ZulaMed.API.Endpoints.MuxLiveStream.EndLiveStream;

public class
    EndLiveStreamCommandHandler : Mediator.ICommandHandler<EndLiveStreamCommand,
        OneOf<Success, NotFound, Error<string>>>
{
    private readonly LiveStreamsApi _api;
    private readonly ZulaMedDbContext _dbContext;
    private readonly ILogger<EndLiveStreamCommandHandler> _logger;

    public EndLiveStreamCommandHandler(LiveStreamsApi api, ZulaMedDbContext dbContext,
        ILogger<EndLiveStreamCommandHandler> logger)
    {
        _api = api;
        _dbContext = dbContext;
        _logger = logger;
    }


    public async ValueTask<OneOf<Success, NotFound, Error<string>>> Handle(EndLiveStreamCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var stream = await _dbContext.Set<LiveStream>()
                .SingleOrDefaultAsync(x => (Guid)x.Id == command.StreamId, cancellationToken);
            if (stream is null)
            {
                _logger.LogWarning("Stream not found: {StreamId}", command.StreamId);
                return new NotFound();
            }
            
            var response = await _api.SignalLiveStreamCompleteAsync(stream.MuxStreamId, cancellationToken);
            if (response is null)
            {
                _logger.LogWarning("Mux stream not found: {MuxStreamId}", stream.MuxStreamId);
                return new NotFound();
            }

            var rows = await _dbContext.Set<LiveStream>()
                .Where(x => (Guid)x.Id == command.StreamId)
                .ExecuteUpdateAsync(calls => calls
                        .SetProperty(x => x.Status, LiveStreamStatus.Disabled), cancellationToken: cancellationToken);

            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while ending the live stream: {Message}", e.Message);
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
        Delete("/stream/{streamId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);


        var response = await _mediator.Send(new EndLiveStreamCommand
        {
            StreamId = req.StreamId,
            UserId = userId
        }, ct);

        await response.Match(
            success => SendOkAsync(ct),
            notFound => SendNotFoundAsync(ct),
            error =>
            {
                AddError("An error occured while ending the live stream");
                return SendErrorsAsync(cancellation: ct);
            });
    }
}