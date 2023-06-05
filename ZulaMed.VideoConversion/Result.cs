using OneOf;
using OneOf.Types;

namespace ZulaMed.VideoConversion;

[GenerateOneOf]
public partial class Result<TValue, TError> : OneOfBase<TValue, Error<TError>>
{
}