using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public readonly partial struct ReplyId
{
    private static Validation Validate(Guid input)
    {
        var isValid = input != Guid.Empty; 
        return isValid ? Validation.Ok : Validation.Invalid("Id cannot be empty");
    }
}

public class Reply
{
    public required ReplyId Id { get; init; }
    
    public required Comment ParentComment { get; init; }
    
    public required Comment ReplyComment { get; init; }
}