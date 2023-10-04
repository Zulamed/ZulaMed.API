using Vogen;

namespace ZulaMed.API.Domain.Accounts.UniversityAccount;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountUniversity
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("University name can't be empty");
    }
}