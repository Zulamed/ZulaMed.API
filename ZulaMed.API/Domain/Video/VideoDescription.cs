using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoDescription
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("description can't be empty");
    }
}