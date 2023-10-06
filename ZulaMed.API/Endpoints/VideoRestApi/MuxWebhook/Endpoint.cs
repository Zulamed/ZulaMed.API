using System.Text.Json;
using FastEndpoints;
using FluentValidation.Results;
using Mediator;

namespace ZulaMed.API.Endpoints.VideoRestApi.MuxWebhook;

public class MuxWebHookValidator : IPreProcessor<Request>
{
    public Task PreProcessAsync(Request req, HttpContext ctx, List<ValidationFailure> failures, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

public class Metadata
{
    public required Guid VideoId { get; set; }
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
        Post();
        PreProcessors(new MuxWebHookValidator());
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var metadata = JsonSerializer.Deserialize<Metadata>(req.Data.Passthrough);
        if (metadata is null)
        {
            AddError("Invalid metadata");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        switch (req.Type)
        {
            case "video.asset.created":
                await _mediator.Send(new AssetCreatedEvent()
                {
                    VideoId = metadata.VideoId
                }, ct);
                break;
            case "video.asset.ready":
                await _mediator.Send(new AssetReadyEvent()
                {
                    VideoId = metadata.VideoId
                }, ct);
                break;
            case "video.upload.canceled":
                await _mediator.Send(new UploadCanceledEvent()
                {
                    VideoId = metadata.VideoId
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