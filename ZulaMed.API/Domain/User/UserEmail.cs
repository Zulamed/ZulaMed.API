using System.Text.RegularExpressions;
using Vogen;

namespace ZulaMed.API.Domain.User;


[ValueObject<string>(Conversions.EfCoreValueConverter)]
public readonly partial struct UserEmail
{
    [GeneratedRegex("^[\\w!#$%&’*+/=?`{|}~^-]+(?:\\.[\\w!#$%&’*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex EmailValidationRegex();
    private static Validation Validate(string input)
    {
        return  EmailValidationRegex().IsMatch(input) ? Validation.Ok : Validation.Invalid("is not a valid email address");
    }
}