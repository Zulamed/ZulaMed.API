using Vogen;

namespace ZulaMed.API.Domain.ViewHistory;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
[Instance("Unspecified", "Guid.Empty")]
public readonly partial struct ViewHistoryId
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}