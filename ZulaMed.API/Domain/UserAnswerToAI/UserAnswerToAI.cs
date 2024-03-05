
namespace ZulaMed.API.Domain.UserAnswerToAI;

public class UserAnswerToAI<TParent>
{
     public Id Id { get; init; } = null!;
     
     public required TParent Parent { get; init; }   
     
     public required UserAnswerContent UserAnswer { get; init; }
     
     public required UserAnswerTime AnswerTime { get; init; }
     
}