using Vogen;

namespace ZulaMed.API.Domain.UserExamAI;


[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public partial class UserExamTime
{
    private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.UtcNow
            ? Validation.Ok 
            : Validation.Invalid("Exam time date must be in the past and in UTC");
    }
}