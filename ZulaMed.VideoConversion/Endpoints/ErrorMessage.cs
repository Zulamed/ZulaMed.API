using System.Net;

namespace ZulaMed.VideoConversion.Endpoints;

public class ErrorMessage
{
    public required string Message { get; set; }
    
    public required int StatusCode { get; set; }
}