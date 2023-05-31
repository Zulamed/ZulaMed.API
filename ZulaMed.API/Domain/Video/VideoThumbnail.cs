using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoThumbnail
{
    private static Validation Validate(string input)
    {
        string pattern = @"\/thumbnails\/[^\/]+\.[a-zA-Z]{3,4}";
        return System.Text.RegularExpressions.Regex.IsMatch(input, pattern) ? Validation.Ok : Validation.Invalid("Thumbnail URL must be in the format '/thumbnails/filename.jpg'.");
    }
}