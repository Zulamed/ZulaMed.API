using Vogen;

namespace ZulaMed.API.Domain.User;


[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserSurname
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("surname can't be empty");
    }
}