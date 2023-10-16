using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
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

public record Request
{
    public required string Type { get; set; }

    public required JsonObject Data { get; init; } = null!;

    [JsonIgnore] public MuxData? MuxData { get; set; }
    
    [JsonIgnore] public string Status { get; set; }
}