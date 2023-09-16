using Vogen;

namespace ZulaMed.API.Domain.User;

[ValueObject<int>(Conversions.EfCoreValueConverter)]
[Instance("Zero", 0)]
public readonly partial struct SubscriberCount
{
}