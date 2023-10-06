using Vogen;

namespace ZulaMed.API.Domain.Accounts;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountPostCode
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("PostalCode can't be empty");
    }
}