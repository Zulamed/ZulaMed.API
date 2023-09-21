using Vogen;

namespace ZulaMed.API.Domain.ViewHistory;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct ViewedAt
{
    private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.UtcNow
            ? Validation.Ok 
            : Validation.Invalid("Viewed date must be in the past and in UTC");
    }
}