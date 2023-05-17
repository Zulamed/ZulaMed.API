using Vogen;

namespace ZulaMed.API.Domain.Video;

[ValueObject<Guid>]
[Instance("Unspecified", "Guid.Empty")]
public readonly partial struct VideoId
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}