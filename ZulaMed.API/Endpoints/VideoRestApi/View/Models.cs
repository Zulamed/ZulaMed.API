using Mediator;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.VideoRestApi.View;

public class Request
{
    public Guid Id { get; set; }    
}

public class ViewCommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public Guid Id { get; set; }
}
