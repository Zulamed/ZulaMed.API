using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public partial class VideoTimelineThumbnail
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input)
            ? Validation.Ok
            : Validation.Invalid("VideoTimelineThumbnail cannot be empty");
    }
}