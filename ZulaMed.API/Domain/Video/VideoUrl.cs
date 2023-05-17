using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>]
public readonly partial struct VideoUrl
{
    private static Validation Validate(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("url can't be empty");
    }
}