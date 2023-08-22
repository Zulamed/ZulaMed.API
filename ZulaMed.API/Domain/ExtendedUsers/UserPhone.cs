using Vogen;

namespace ZulaMed.API.Domain.ExtendedUsers;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserPhone
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Phone can't be empty");
    }
}