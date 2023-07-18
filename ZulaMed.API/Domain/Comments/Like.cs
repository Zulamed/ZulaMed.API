using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct Like 
{
    private static Validation Validate(int input)
    {
        var isValid = input > 0; 
        return isValid ? Validation.Ok : Validation.Invalid("Like cannot be less than 0");
    }
}