using Vogen;
using System.Text.RegularExpressions;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoUrl
{
    [GeneratedRegex(@"^\/[a-fA-F0-9]{8}-(?:[a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex UrlValidationRegex();

    private static Validation Validate(string input)
    {
        return UrlValidationRegex().IsMatch(input) ? Validation.Ok : Validation.Invalid("URL must be in the format '/GUID'");
    }
}