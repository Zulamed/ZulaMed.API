using Vogen;

namespace ZulaMed.API.Domain.Playlist;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct PlaylistName
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("Playlist Name can't be empty");
    }
}