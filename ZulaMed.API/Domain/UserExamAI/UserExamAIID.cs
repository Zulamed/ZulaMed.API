using Vogen;


namespace ZulaMed.API.Domain.UserExamAI;


[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public partial class UserExamAIID 
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("User Exam Id cannot be empty");
    }
}