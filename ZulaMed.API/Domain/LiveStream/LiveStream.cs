using Mux.Csharp.Sdk.Model;

namespace ZulaMed.API.Domain.LiveStream;

public class LiveStream
{
    public required LiveStreamId Id { get; set; }
    
    public required Video.Video RelatedVideo { get; set; } 
    
    public required LiveStreamStatus Status { get; set; } 
}