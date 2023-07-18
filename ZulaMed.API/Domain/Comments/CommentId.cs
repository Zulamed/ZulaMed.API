using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public readonly partial struct CommentId
{
    private static Validation Validate(Guid input)
    {
        var isValid = input != Guid.Empty; 
        return isValid ? Validation.Ok : Validation.Invalid("id cannot be empty");
    }
}