using System.Text.RegularExpressions;
using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoThumbnail
{
    
    [GeneratedRegex(@"^\/[a-fA-F0-9]{8}-(?:[a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}\/[^\/]+\.[a-zA-Z]{3,4}$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex ThumbnailValidationRegex();
    
    private static Validation Validate(string input)
    {
        return ThumbnailValidationRegex().IsMatch(input) ? Validation.Ok : Validation.Invalid("Thumbnail URL must be in the format '/GUID/filename.***'.");
    }
}