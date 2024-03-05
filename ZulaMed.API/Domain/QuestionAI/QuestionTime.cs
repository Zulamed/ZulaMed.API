using Vogen;

namespace ZulaMed.API.Domain.QuestionAI;



[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]

public partial class QuestionTime
{
      private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.UtcNow
            ? Validation.Ok 
            : Validation.Invalid("Liked date must be in the past and in UTC");
    }
}