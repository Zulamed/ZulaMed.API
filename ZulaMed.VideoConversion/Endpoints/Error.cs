using System.Net;

namespace ZulaMed.VideoConversion.Endpoints;

public class Error
{
    public required string ErrorMessage { get; set; }
    
    public required int StatusCode { get; set; }
}