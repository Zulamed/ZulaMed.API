using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>]
public readonly partial struct VideoUrl
{
    private static Validation Validate(string input)
    {
        return Uri.IsWellFormedUriString(input, UriKind.Absolute) ? Validation.Ok : Validation.Invalid("url must to be valid");
    }
}