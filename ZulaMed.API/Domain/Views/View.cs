namespace ZulaMed.API.Domain.Views;

public class View
{
    public Id Id { get; init; } = null!;
    
    public ViewedAt ViewedAt { get; init; } 
    
    public required Video.Video Video { get; init; }
    
    public required User.User ViewedBy { get; init; }
}