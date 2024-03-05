using ZulaMed.API.Domain.UserAnswerToAI;

namespace ZulaMed.API.Domain.QuestionAI;

public class QuestionAI<TParent> 
{
      public Id Id { get; init; } = null!;
      
      public required TParent Parent { get; init; }
         
      public required QuestionContent QuestionContent { get; init; }

      public required QuestionTime QuestionTime { get; init; }
      
      public UserAnswerToAI<QuestionAI<UserExamAI.UserExamAI>> Answer { get; init; }
      
      
}