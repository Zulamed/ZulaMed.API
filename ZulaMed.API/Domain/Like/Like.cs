using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Domain.Like;

public class Like<TParent>
{
    public Id Id { get; init; } = null!;
    
    public required LikedAt LikedAt { get; init; } 
    
    public required TParent Parent { get; init; }
    
    public required User.User LikedBy { get; init; }
}
