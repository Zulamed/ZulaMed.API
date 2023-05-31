using Vogen;

namespace ZulaMed.API.Domain.SpecialtyGroup;

[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct SpecialtyGroupName
{
    private static Validation Validate(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("Name can't be empty");
    }
}