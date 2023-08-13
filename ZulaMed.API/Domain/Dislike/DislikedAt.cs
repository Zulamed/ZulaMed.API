using Vogen;

namespace ZulaMed.API.Domain.Dislike;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct DislikedAt
{
    private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.UtcNow
            ? Validation.Ok 
            : Validation.Invalid("Disliked date must be in the past and in UTC");
    }
}