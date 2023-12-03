using FastEndpoints;
using Mediator;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;


public class Request
{
    [QueryParam]
    public string? Title { get; init; }
    [QueryParam]
    public bool? Own { get; init; }
    
    [QueryParam]
    public bool? Liked { get; init; }
    
    [QueryParam]
    public bool? Subscriptions { get; init; }
    
    [QueryParam]
    public int Page { get; init; } = 1;
    
    [QueryParam]
    public int PageSize { get; init; } = 10;
}

public class UserDTO
{
   public required Guid Id { get; init; }
   
   public string? ProfilePictureUrl { get; init; }
   
   public required string Username { get; init; } 
}

public class AllVideoDto
{
    public required VideoDTO Video { get; init; }
    public required UserDTO User { get; init; }
}

public class Response
{
    public required AllVideoDto[] Videos { get; init; }
    
    public required int TotalCount { get; init; }
}

