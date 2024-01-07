using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        var logger = ctx.Resolve<ILogger<Binder>>();

        // so basically because Mux's sdk is written with RestSharp which uses NewtonsoftJson,
        // i'll have to use NewtonsoftJson for deserialization
        // When i tried to use System.Text.Json, it was throwing an error
        // So, i just get the data as a JsonObject and then deserialize it with NewtonsoftJson
        var body = JsonSerializer.Deserialize<JsonObject>(req.Content, ctx.SerializerOptions)!;

        req.Type = (string)body["type"]!;
        req.Data = body["data"]!.AsObject();


        // So because Mux SDK's Asset class had started_at as a string instead of a object,
        // the deserialization was giving an exception when the property was present.
        // The solution was to just remove the property from the json object so that deserialization can happen properly
        var recordingTimes = req.Data["recording_times"];
        if (recordingTimes is not null)
        {
            foreach (var jsonNode in recordingTimes.AsArray())
            {
                jsonNode!.AsObject().Remove("started_at");
            }
        }

        req.Status = (string)req.Data["status"]!;
        try
        {
            if (req.Type.StartsWith("video.asset"))
            {
                var asset = JsonConvert.DeserializeObject<Asset>(req.Data.ToString());
                req.MuxData = new MuxData
                {
                    Data = asset!,
                    Metadata = JsonSerializer.Deserialize<Metadata>(asset!.Passthrough, ctx.SerializerOptions)!
                };
            }

            else if (req.Type.StartsWith("video.upload"))
            {
                var upload = JsonConvert.DeserializeObject<Upload>(req.Data.ToString());
                req.MuxData = new MuxData
                {
                    Data = upload!.NewAssetSettings,
                    Metadata = JsonSerializer.Deserialize<Metadata>(upload.NewAssetSettings.Passthrough,
                        ctx.SerializerOptions)!
                };
            }
            else if (req.Type.StartsWith("video.live_stream"))
            {
                var liveStream = JsonConvert.DeserializeObject<LiveStream>(req.Data.ToString());
                req.MuxData = new MuxData
                {
                    Data = liveStream!,
                    Metadata = JsonSerializer.Deserialize<Metadata>(liveStream!.Passthrough, ctx.SerializerOptions)!
                };
            }
        }
        catch (JsonReaderException e)
        {
            logger.LogError("An error occured while deserializing Mux webhook data: {Message}", e.Message);
            throw;
        }


        return req;
    }
}

public class MuxWebHookValidator : IPreProcessor<Request>
{
    // public async Task PreProcessAsync(Request req, HttpContext ctx, List<ValidationFailure> failures,
    //     CancellationToken ct)
    // {
    //     var webhookValidator = ctx.Resolve<IMuxWebhookValidator>();
    //
    //     var values = req.MuxSignature.Split(",");
    //     var timestamp = values[0].Replace("t=", "");
    //     var signatureValue = values[1].Replace("v1=", "");
    //     var body = req.Content;
    //
    //     var isValid = webhookValidator.Validate(signatureValue, timestamp, body);
    //
    //     if (!isValid)
    //     {
    //         var logger = ctx.Resolve<ILogger<MuxWebHookValidator>>();
    //         logger.LogWarning("Invalid signature for Mux webhook");
    //         failures.Add(new ValidationFailure("mux-signature", "Invalid signature"));
    //         await ctx.Response.SendErrorsAsync(failures, cancellation: ct);
    //     }
    // }

    public async Task PreProcessAsync(IPreProcessorContext<Request> ctx, CancellationToken ct)
    {
        var webhookValidator = ctx.HttpContext.Resolve<IMuxWebhookValidator>();
        
        var values = ctx.Request.MuxSignature.Split(",");
        var timestamp = values[0].Replace("t=", "");
        var signatureValue = values[1].Replace("v1=", "");
        var body = ctx.Request.Content;
        
        var isValid = webhookValidator.Validate(signatureValue, timestamp, body);
        
        if (!isValid)
        {
            var logger = ctx.HttpContext.Resolve<ILogger<MuxWebHookValidator>>();
            logger.LogWarning("Invalid signature for Mux webhook");
            ctx.ValidationFailures.Add(new ValidationFailure("mux-signature", "Invalid signature"));
            await ctx.HttpContext.Response.SendErrorsAsync(ctx.ValidationFailures, cancellation: ct);
        }
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
        var capitalized = muxEventType.Split(".").Select(x => x.Capitalize()).ToArray();
        if (capitalized[1].Contains('_'))
        {
            var split = capitalized[1].Split("_").Select(x => x.Capitalize());
            capitalized[1] = string.Join("", split);
        }

        if (capitalized[2].Contains('_'))
        {
            var split = capitalized[2].Split("_").Select(x => x.Capitalize());
            capitalized[2] = string.Join("", split);
        }

        var name = $"{capitalized[1]}{capitalized[2]}Event";
        return Type.GetType($"ZulaMed.API.Endpoints.MuxWebhook.{name}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        _logger.LogInformation("Mux Webhook: {Type} - type", req.Type);
        var eventType = TransformToEventType(req.Type);
        if (eventType is null)
        {
            // i'm gonna send a 200 response because i don't want Mux to keep sending the webhook
            await SendOkAsync(ct);
            return;
        }

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
