using Vogen;

namespace ZulaMed.API.Domain.UserExamAI;


[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public partial class Grade
{
    private static Validation Validate(int input)
    {
        return input > 0 ? Validation.Ok : Validation.Invalid("Grade cannot be less than 0");
    }
}



