using Vogen;

namespace ZulaMed.API.Domain.ExtendedUsers.HospitalUser;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserHospital
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("HospitalName can't be empty");
    }
}