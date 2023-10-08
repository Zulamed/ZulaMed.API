using System.Text.Json;
using FastEndpoints;
using FluentValidation.Results;
using Mediator;

namespace ZulaMed.API.Endpoints.VideoRestApi.MuxWebhook;

public class Binder : RequestBinder<Request>
{
    public override async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken cancellation)
    {
        var req = await base.BindAsync(ctx, cancellation);

        req.MuxData.Metadata = req.Data.TryGetValue("passthrough", out var passthroughJson)
            ? JsonSerializer.Deserialize<Metadata>(passthroughJson.GetString()!, ctx.SerializerOptions)
            : null;

        req.MuxData.Status = req.Data.TryGetValue("status", out var statusJson)
            ? statusJson.GetString()
            : null;
       
        req.MuxData.PlaybackId = req.Data.TryGetValue("playback_ids", out var playbackIdsJson)
            ? playbackIdsJson.EnumerateArray().First().GetProperty("id").GetString()
            : null;
        
        return req;
    }
}

public class MuxWebHookValidator : IPreProcessor<Request>
{
    public Task PreProcessAsync(Request req, HttpContext ctx, List<ValidationFailure> failures,
        CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public Endpoint(IMediator mediator, ILogger<Endpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/video/mux");
        PreProcessors(new MuxWebHookValidator());
        RequestBinder(new Binder());
        AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        _logger.LogInformation("{Type} - type", req.Type);
        _logger.LogInformation("{Status} - status", req.MuxData.Status);
        if (req.MuxData.Metadata is not null)
            _logger.LogInformation("{VideoId} - video id", req.MuxData.Metadata.VideoId);
        

        switch (req.Type)
        {
            case "video.asset.created":
                if (req.MuxData.Status == "ready")
                {
                    break;
                }
                await _mediator.Send(new AssetCreatedEvent
                {
                    VideoId = req.MuxData.Metadata!.VideoId
                }, ct);
                break;
            case "video.asset.ready":
                _logger.LogInformation("{PlaybackId} - playback id", req.MuxData.PlaybackId);
                if (req.MuxData.PlaybackId is null)
                {
                    AddError("PlaybackId was null");
                    await SendErrorsAsync(cancellation: ct);
                    return;
                }
                await _mediator.Send(new AssetReadyEvent
                {
                    VideoId = req.MuxData.Metadata!.VideoId,
                    PlaybackId =  req.MuxData.PlaybackId
                }, ct);
                break;
            case "video.upload.canceled":
                await _mediator.Send(new UploadCanceledEvent
                {
                    VideoId = req.MuxData.Metadata!.VideoId
                }, ct);
                break;
            default:
            {
                AddError("Invalid webhook type");
                await SendErrorsAsync(cancellation: ct);
                return;
            }
        }

        await SendOkAsync(ct);
    }
}