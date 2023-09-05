using Mediator;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API;

[GenerateOneOf]
public partial class Result<TValue, TError> : OneOfBase<TValue, Error<TError>>, ICommand<Unit>
{
}