using Vogen;
using System.Text.RegularExpressions;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public partial class VideoUrl
{
    // [GeneratedRegex(@"^https:\/\/stream\.mux\.com\/[^\/]+\.m3u8$", 
    //     RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    // private static partial Regex UrlValidationRegex();

    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("URL must be in the format '/GUID'");
    }
}