using Vogen;

namespace ZulaMed.API.Domain.User;


[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserCountry
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("country can't be empty");
    }
}