using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public partial class VideoTitle
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("title can't be empty");
    }
}