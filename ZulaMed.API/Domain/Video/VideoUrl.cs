using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoUrl
{
    private static Validation Validate(string input)
    {
        string pattern = @"\/videos\/[a-fA-F0-9]{8}-(?:[a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}";
        return System.Text.RegularExpressions.Regex.IsMatch(input, pattern) ? Validation.Ok : Validation.Invalid("URL must be in the format '/videos/GUID'");
    }
}