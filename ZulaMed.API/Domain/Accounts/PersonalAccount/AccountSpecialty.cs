using Vogen;

namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountSpecialty
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Personal's Specialty can't be empty");
    }
}