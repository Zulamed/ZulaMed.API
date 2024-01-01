using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<bool>(Conversions.EfCoreValueConverter)]
public readonly partial struct IsVerified
{
}