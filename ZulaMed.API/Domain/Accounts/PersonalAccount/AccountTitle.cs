using Vogen;

namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountTitle
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Personal's Title can't be empty");
    }
}