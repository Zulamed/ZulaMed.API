using Vogen;

namespace ZulaMed.API.Domain.Accounts;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountAddress
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Address can't be empty");
    }
}