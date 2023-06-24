using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct VideoPublishedDate
{
    private static Validation Validate(DateTime input)
    {
        return DateTime.Compare(input, DateTime.Today.ToUniversalTime()) >= 0
            ? Validation.Ok
            : Validation.Invalid("published date can't be in the past");
    }
}