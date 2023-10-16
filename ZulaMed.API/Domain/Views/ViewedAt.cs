using Vogen;

namespace ZulaMed.API.Domain.Views;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct ViewedAt
{
    private static Validation Validate(DateTime input)
    {
        var isValid = DateTime.UtcNow > input && input.Kind == DateTimeKind.Utc; 
        return isValid ? Validation.Ok : Validation.Invalid("ViewedAt must be in the past and in UTC");
    }
}