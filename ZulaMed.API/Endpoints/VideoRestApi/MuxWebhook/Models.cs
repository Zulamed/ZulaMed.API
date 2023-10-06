using Mediator;
using Mux.Csharp.Sdk.Model;

namespace ZulaMed.API.Endpoints.VideoRestApi.MuxWebhook;

public class Request 
{
    public required string Type { get; set; }
    
    public required Asset Data { get; set; }
}
