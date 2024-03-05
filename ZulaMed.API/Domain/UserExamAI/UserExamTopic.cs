using Vogen;

namespace ZulaMed.API.Domain.UserExamAI;



[ValueObject<string>(Conversions.EfCoreValueConverter)]
public partial class UserExamTopic
{
    
    private static Validation Validate(string input)
    {
        return !string.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("Exam topic can't be empty");
    }
}


