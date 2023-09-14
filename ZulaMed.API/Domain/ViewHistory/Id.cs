using Vogen;

namespace ZulaMed.API.Domain.ViewHistory;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public partial class Id
{
}