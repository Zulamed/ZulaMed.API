using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct VideoLike
{
    private static Validation Validate(int input)
    {
        return input > 0 ? Validation.Ok : Validation.Invalid("like cannot be less than 0");
    }
}
    
