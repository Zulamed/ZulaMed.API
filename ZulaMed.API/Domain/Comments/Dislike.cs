using Vogen;

namespace ZulaMed.API.Domain.Comments;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct Dislike 
{
    private static Validation Validate(int input)
    {
        var isValid = input > 0; 
        return isValid ? Validation.Ok : Validation.Invalid("Dislike cannot be less than 0");
    }
}