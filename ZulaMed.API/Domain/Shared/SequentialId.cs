using Vogen;

namespace ZulaMed.API.Domain.Shared;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
public readonly partial struct SequentialId
{
    private static Validation Validate(int input)
    {
        var isValid = input > 0; 
        return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
    }
}