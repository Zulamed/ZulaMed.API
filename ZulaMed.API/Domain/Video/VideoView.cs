using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<long>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct VideoView
{
    private static Validation Validate(long input)
    {
        return input > 0 ? Validation.Ok : Validation.Invalid("views count must be greater than zero");
    }
}