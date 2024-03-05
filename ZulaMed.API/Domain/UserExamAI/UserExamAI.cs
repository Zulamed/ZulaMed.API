using ZulaMed.API.Domain.QuestionAI;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.UserAnswerToAI;

namespace ZulaMed.API.Domain.UserExamAI;

public class UserExamAI
{
        public required UserExamAIID Id { get; set; }
        
        public Grade ExamGrade { get; init; } = Grade.Zero;
        
        public UserId UserId { get; set; }
        public required User.User User { get; init; }
        
        public required UserExamStartTime ExamStartTime { get; init; }
        
        public required UserExamEndTime ExamEndTime { get; init; }

        public required UserExamTime ExamTime { get; init; }

        public required UserExamTopic ExamTopic { get; init; }

        public List<QuestionAI<UserExamAI>> Questions { get; init; } = new();
        
       // public List<UserAnswerToAI<UserExamAI>> Answers { get; init; } = new(); 
        
}
