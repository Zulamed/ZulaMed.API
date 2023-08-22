using Vogen;

namespace ZulaMed.API.Domain.ExtendedUsers;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserPostCode
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("PostalCode can't be empty");
    }
}