using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct Description 
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Description can't be null");
    }
}