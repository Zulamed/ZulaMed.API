using Vogen;

namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

[ValueObject<DateOnly>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountBirthDate
{
    private static Validation Validate(DateOnly input)
    {
        return input < DateOnly.FromDateTime(DateTime.UtcNow)
            ? Validation.Ok
            : Validation.Invalid("birth date can't be in the past");
    }
}