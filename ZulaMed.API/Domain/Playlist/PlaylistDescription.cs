using Vogen;

namespace ZulaMed.API.Domain.Playlist;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct PlaylistDescription
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("Playlist Description can't be empty");
    }
}