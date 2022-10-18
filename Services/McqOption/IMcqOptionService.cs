using quizz.Models;

namespace quizz.Services;

public interface IMcqOptionService
{
    ValueTask<Result<List<McqOption>>> GetAllOptionsByQuestionIdAsync(ulong questionId);  
    ValueTask<Result<McqOption>> CreateOptionsAsync(List<McqOption> models, ulong questionId);
    ValueTask<Result<McqOption>> RemoveOptionsByQuestionIdAsync(ulong questionId); 
}