using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserUniversity
{
    private static Validation Validate(string input)
    {
        return !String.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("university can't be empty");
    }
}