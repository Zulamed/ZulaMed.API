using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserWorkPlace
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrEmpty(input) ? Validation.Ok : Validation.Invalid("workplace can't be empty");
    }
}