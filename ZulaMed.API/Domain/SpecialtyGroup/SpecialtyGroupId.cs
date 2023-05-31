using Microsoft.EntityFrameworkCore;
using Vogen;

namespace ZulaMed.API.Domain.SpecialtyGroup;


[ValueObject<int>(Conversions.EfCoreValueConverter)]
public readonly partial struct SpecialtyGroupId
{
    private static Validation Validate(int input)
    {
        return input > 0 ? Validation.Ok : Validation.Invalid("Id can't be lower than or equal to 0");
    }
}