using Vogen;

namespace ZulaMed.API.Domain.LiveStream;

[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
public partial class LiveStreamId 
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("LiveStreamId cannot be empty");
    }
}