using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct VideoDislike
{
    private static Validation Validate(int input)
    {
        return input > 0 ? Validation.Ok : Validation.Invalid("dislike count must be greater than zero");
    }
}