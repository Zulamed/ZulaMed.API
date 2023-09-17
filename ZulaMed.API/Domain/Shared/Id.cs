using Vogen;

namespace ZulaMed.API.Domain.Shared;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public partial class Id
{
    private static Validation Validate(Guid input)
    {
        var isValid = Guid.Empty != input; 
        return isValid ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}