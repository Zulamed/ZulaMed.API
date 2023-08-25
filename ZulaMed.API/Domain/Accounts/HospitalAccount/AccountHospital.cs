using Vogen;

namespace ZulaMed.API.Domain.Accounts.HospitalAccount;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountHospital
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("HospitalName can't be empty");
    }
}