using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<DateTime>(Conversions.EfCoreValueConverter)]
public readonly partial struct RegistrationTime
{
}