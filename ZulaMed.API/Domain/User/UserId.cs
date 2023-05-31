using Vogen;
using ZulaMed.API.Endpoints.VideoRestApi.Post;

namespace ZulaMed.API.Domain.User;



[ValueObject<Guid>(Conversions.EfCoreValueConverter)]
[Instance("Unspecified", "Guid.Empty")]
public readonly partial struct UserId
{
    private static Validation Validate(Guid input)
    {
        return input != Guid.Empty ? Validation.Ok : Validation.Invalid("Id can't be empty");
    }
}