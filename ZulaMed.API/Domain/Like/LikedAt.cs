using Vogen;

namespace ZulaMed.API.Domain.Like;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct LikedAt
{
    private static Validation Validate(DateTime input)
    {
        return input.Kind == DateTimeKind.Utc && input < DateTime.Now 
            ? Validation.Ok 
            : Validation.Invalid("Liked date must be in the past and in UTC");
    }
}