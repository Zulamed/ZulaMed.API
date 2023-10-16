using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using FastEndpoints;
using FluentValidation;
using Mux.Csharp.Sdk.Model;

namespace ZulaMed.API.Endpoints.MuxWebhook;

public class Metadata
{
    public required Guid VideoId { get; set; }
}

public class MuxData
{
    public required Metadata Metadata { get; set; }

    public required Asset Data { get; set; }
}

public record Request : IPlainTextRequest
{
    public required string Content { get; set; }

    [FromHeader("mux-signature")] 
    public string MuxSignature { get; set; } = null!;

    [JsonIgnore] public string Type { get; set; }

    [JsonIgnore] public JsonObject Data { get; set; } = null!;

    [JsonIgnore] public MuxData? MuxData { get; set; }

    [JsonIgnore] public string Status { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.MuxSignature).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}