using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>]
public readonly partial struct VideoThumbnail
{
    private static Validation Validate(string input)
    {
        return !String.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("thumbnail url can't be empty");
    }
}