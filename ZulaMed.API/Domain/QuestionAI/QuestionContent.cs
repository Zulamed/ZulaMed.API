using Vogen;

namespace ZulaMed.API.Domain.QuestionAI;


[ValueObject<string>(Conversions.EfCoreValueConverter)]

public partial class QuestionContent
{
    private static Validation Validate(string input)
    {
        var isValid = !string.IsNullOrWhiteSpace(input);
        return isValid ? Validation.Ok : Validation.Invalid("content cannot be empty");
    }
}