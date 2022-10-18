using quizz.Models;
using quizz.Models.Quiz;

namespace quizz.Services;

public interface IQuizService
{
    ValueTask<Result<List<Quiz>>> GetAllQuizzesPaginatedAsync(int page, int limit);
    ValueTask<Result<Quiz>> GetByIdAsync(ulong id);
    ValueTask<Result<Quiz>> RemoveByIdAsync(ulong id);
    ValueTask<Result<Quiz>> CreateAsync(string title, string description, DateTime startTime, DateTime endTime, string? password = null);
    ValueTask<Result<Quiz>> UpdateAsync(ulong id, string title, string description, DateTime startTime, DateTime endTime, string? password = null);
    ValueTask<bool> ExistsAsync(ulong id);
}