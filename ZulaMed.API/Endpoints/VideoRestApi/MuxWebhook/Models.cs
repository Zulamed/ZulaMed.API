using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZulaMed.API.Endpoints.VideoRestApi.MuxWebhook;

public class Metadata
{
    public required Guid VideoId { get; set; }
}

public class MuxData
{
    public Metadata? Metadata { get; set; }
}

public class Request
{
    public required string Type { get; set; }
    public required Dictionary<string, JsonElement> Data { get; init; }

    [JsonIgnore] public MuxData MuxData { get; set; } = new();
}