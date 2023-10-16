using Vogen;

namespace ZulaMed.API.Domain.Views;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public partial class Id
{
}