namespace ZulaMed.API.Domain.Dislike;

public class Dislike<TParent>
{
    public Id Id { get; init; } = null!;
    public required DislikedAt DislikedAt { get; init; }
    public required TParent Parent { get; init; }
    public required User.User DislikedBy { get; init; }
}