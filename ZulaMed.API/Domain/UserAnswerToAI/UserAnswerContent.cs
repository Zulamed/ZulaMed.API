using Vogen;

namespace ZulaMed.API.Domain.UserAnswerToAI;


[ValueObject<string>(Conversions.EfCoreValueConverter)]

public partial class UserAnswerContent
{
    private static Validation Validate(string input)
    {
        var isValid = !string.IsNullOrWhiteSpace(input);
        return isValid ? Validation.Ok : Validation.Invalid("content cannot be empty");
    }
}