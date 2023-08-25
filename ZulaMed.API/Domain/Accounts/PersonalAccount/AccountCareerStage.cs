using Vogen;

namespace ZulaMed.API.Domain.Accounts.PersonalAccount;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct AccountCareerStage
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Personal's Career Stage can't be empty");
    }
}

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct PlaceOfWork
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Personal's PlaceOfWork can't be empty");
    }
}