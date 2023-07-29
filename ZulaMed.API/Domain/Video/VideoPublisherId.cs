using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoPublisherId
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}