using FastEndpoints;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Domain.UserExamAI;

namespace ZulaMed.API.Endpoints.ChatAI.StartExamAI;



public class Request
{
    [FromClaim]
    public Guid UserId { get; init; }
    
  
}


public class StartExamAICommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public required UserExamTopic SelectedTopic { get; init; }
    public required Guid UserId { get; init; }
}