using FastEndpoints;
using Mediator;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;


public class Request
{
    [QueryParam]
    public string? Title { get; init; }
}

public class Response
{
    public required VideoDTO[] Videos { get; init; }
}

