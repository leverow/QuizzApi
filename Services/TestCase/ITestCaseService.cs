using quizz.Models;

namespace quizz.Services;

public interface ITestCaseService 
{
    ValueTask<Result<FileStream>> GetAllTestCasesAsync(string filename); 
    ValueTask<Result<IEnumerable<string>>> GetTestCaseByQuestionIdAsync(FileStream stream);
    ValueTask<Result<TestCase>> CreateTestCasesAsync(IFormFile file, ulong questionId);
}