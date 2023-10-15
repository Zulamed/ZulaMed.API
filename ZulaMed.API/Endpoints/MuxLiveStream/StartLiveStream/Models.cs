using System.Runtime.InteropServices;
using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.MuxLiveStream.StartLiveStream;

public class Request
{
    // to be removed
    public Guid UserId { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public string? VideoThumbnail { get; init; } 
}

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UserNotFound
{
}

public class Response
{
    public required Guid StreamId { get; init; }
    
    public required string PlaybackUrl { get; init; }
    
    public required string StreamKey { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
       RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("Name is required");
    } 
}