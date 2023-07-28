using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
[Instance("Empty", "null")]
public readonly partial struct PhotoUrl
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("PhotoUrl can't be empty");
    }
}