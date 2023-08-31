using Vogen;

namespace ZulaMed.API.Domain.Playlist;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
[Instance("Unspecified", "Guid.Empty")]
public readonly partial struct PlaylistId
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}