using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct CommentSentDate
{
    private static Validation Validate(DateTime input)
    {
        var isValid = input < DateTime.UtcNow; 
        return isValid ? Validation.Ok : Validation.Invalid("Sent date can't be in the past");
    }
}