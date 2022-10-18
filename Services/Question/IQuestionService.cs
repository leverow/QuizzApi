using quizz.Models;

namespace quizz.Services;

public interface IQuestionService
{
    ValueTask<Result<List<Question>>> GetAllQuestionsAsync(int page = 1, int limit = 100, string? topic = null, string? search = "", Models.Topic.ETopicDifficulty difficulty = Models.Topic.ETopicDifficulty.Beginner);
    ValueTask<Result<Question>> GetByIdAsync(ulong id);
    ValueTask<Result<Question>> FindByTitleAsync(string title);
    ValueTask<Result<Question>> CreateAsync(string title, string description, EQuestionType type, uint TimeAllowed, ulong topicId);
    ValueTask<Result<Question>> UpdateAsync( ulong id, string title, string description, EQuestionType type, uint TimeAllowed, ulong topicId);
    ValueTask<Result<Question>> RemoveByIdAsync(ulong id);
    ValueTask<bool> ExistsAsync(ulong id);
}