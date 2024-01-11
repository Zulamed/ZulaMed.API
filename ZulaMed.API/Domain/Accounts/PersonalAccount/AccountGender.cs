using Vogen;

namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

public enum Gender
{
    Male,
    Female,
    Other
}

[ValueObject<Gender>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountGender
{
}