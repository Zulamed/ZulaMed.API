using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct CommentContent
{
    private static Validation Validate(string input)
    {
        var isValid = !string.IsNullOrWhiteSpace(input);
        return isValid ? Validation.Ok : Validation.Invalid("content cannot be empty");
    }
}