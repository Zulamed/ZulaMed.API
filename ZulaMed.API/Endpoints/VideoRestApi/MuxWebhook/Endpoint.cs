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

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video/mux/webhook");
        PreProcessors(new MuxWebHookValidator());
        RequestBinder(new Binder());
        AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (req.MuxData.Metadata is null)
        {
            AddError("Metadata was null");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        switch (req.Type)
        {
            case "video.asset.created":
                await _mediator.Send(new AssetCreatedEvent
                {
                    VideoId = req.MuxData.Metadata.VideoId
                }, ct);
                break;
            case "video.asset.ready":
                await _mediator.Send(new AssetReadyEvent
                {
                    VideoId = req.MuxData.Metadata.VideoId
                }, ct);
                break;
            case "video.upload.canceled":
                await _mediator.Send(new UploadCanceledEvent
                {
                    VideoId = req.MuxData.Metadata.VideoId
                }, ct);
                break;
            default:
            {
                AddError("Invalid webhook type");
                await SendErrorsAsync(cancellation: ct);
                break;
            }
        }

        await SendOkAsync(ct);
    }
}