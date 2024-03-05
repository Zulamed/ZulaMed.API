using Vogen;

namespace ZulaMed.API.Domain.UserAnswerToAI;


[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]

public partial class UserAnswerTime
{  
    private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.UtcNow
            ? Validation.Ok 
            : Validation.Invalid("User Answer Time must be in the past and in UTC");
    }
}