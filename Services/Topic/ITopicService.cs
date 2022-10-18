using quizz.Models;
using quizz.Models.Topic;

namespace quizz.Services;

public interface ITopicService
{
    ValueTask<Result<List<Topic>>> GetAllPaginatedTopicsAsync(int page, int limit);
    ValueTask<Result<Topic>> GetByIdAsync(ulong id);
    ValueTask<Result<Topic>> FindByNameAsync(string name);
    ValueTask<Result<Topic>> RemoveByIdAsync(ulong id);
    ValueTask<Result<Topic>> CreateAsync(string name, string description, ETopicDifficulty difficulty);
    ValueTask<Result<Topic>> UpdateAsync(ulong id, string name, string description, ETopicDifficulty difficulty);
    ValueTask<bool> ExistsAsync(ulong id);
}