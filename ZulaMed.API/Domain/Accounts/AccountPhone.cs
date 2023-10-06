using Vogen;

namespace ZulaMed.API.Domain.Accounts;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountPhone
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Phone can't be empty");
    }
}