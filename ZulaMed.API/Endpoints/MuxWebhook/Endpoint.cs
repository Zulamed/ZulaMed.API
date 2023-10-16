using System.Text.Json;
using FastEndpoints;
using FluentValidation.Results;
using Mediator;
using Mux.Csharp.Sdk.Model;
using Newtonsoft.Json;
using ZulaMed.API.Extensions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ZulaMed.API.Endpoints.MuxWebhook;

public class Binder : RequestBinder<Request>
{
    public override async ValueTask<Request> BindAsync(BinderContext ctx, CancellationToken cancellation)
    {
        var req = await base.BindAsync(ctx, cancellation);

        // so basically because Mux's sdk is written with RestSharp which uses NewtonsoftJson,
        // i'll have to use NewtonsoftJson for deserialization
        // When i tried to use System.Text.Json, it was throwing an error
        
        // So, i just get the data as a JsonObject and then deserialize it with NewtonsoftJson
        
        
        req.Status = (string)req.Data["status"]!;
        
        if (req.Type.StartsWith("video.asset"))
        {
            var asset = JsonConvert.DeserializeObject<Asset>(req.Data.ToString());
            req.MuxData = new MuxData
            {
                Data = asset!, 
                Metadata = JsonSerializer.Deserialize<Metadata>(asset!.Passthrough, ctx.SerializerOptions)!
            };
        }
        
        if (req.Type.StartsWith("video.upload"))
        {
            var upload = JsonConvert.DeserializeObject<Upload>(req.Data.ToString());
            req.MuxData = new MuxData
            {
                Data = upload!.NewAssetSettings,
                Metadata = JsonSerializer.Deserialize<Metadata>(upload.NewAssetSettings.Passthrough,
                    ctx.SerializerOptions)!
            };
        }

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


    private static Type? TransformToEventType(string muxEventType)
    {
        var capitalized = muxEventType.Split(".").Select(x => x.Capitalize());
        var joined = string.Join("", capitalized);
        return Type.GetType("ZulaMed.API.Endpoints.MuxWebhook." + joined + "Event");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var eventType = TransformToEventType(req.Type);
        if (eventType is null)
        {
            AddError("Invalid webhook type");
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        _logger.LogInformation("Mux Webhook: {Type} - type", req.Type);
        _logger.LogInformation("Mux Webhook: {Status} - status", req.Status);
        _logger.LogInformation("Mux Webhook: {VideoId} - video id", req.MuxData!.Metadata.VideoId);

        var instance = (IMuxEvent)Activator.CreateInstance(eventType)!;

        instance.Data = req.MuxData;

        var result = await _mediator.Send(instance, ct);

        await result.Match(
            r => SendOkAsync(r, ct),
            e =>
            {
                AddError("Error processing webhook");
                return SendErrorsAsync(cancellation: ct);
            });
    }
}