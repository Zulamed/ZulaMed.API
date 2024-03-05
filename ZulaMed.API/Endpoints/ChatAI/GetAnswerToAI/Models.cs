using FastEndpoints;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Domain.UserAnswerToAI;
using ZulaMed.API.Domain.UserExamAI;

namespace ZulaMed.API.Endpoints.ChatAI.GetAnswerToAI;

public class Request
{
    [FromClaim]
    public Guid UserId { get; init; }
    
  
}


public class GetAnswerToAICommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public required UserAnswerContent UserAnswer { get; init; }
    public required Guid UserId { get; init; }
}