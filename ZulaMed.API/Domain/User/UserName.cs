using Microsoft.EntityFrameworkCore;
using Vogen;

namespace ZulaMed.API.Domain.User;


[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserName
{
    private static Validation Validate(string input)
    {
        return !string.IsNullOrWhiteSpace(input) ? Validation.Ok : Validation.Invalid("User name can't be empty");
    }
}