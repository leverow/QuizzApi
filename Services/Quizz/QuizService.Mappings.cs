using quizz.Models.Quiz;

namespace quizz.Services;

public partial class QuizService
{
    public static Quiz ToModel(Entities.Quiz entity)
    => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Title = entity.Title,
        Description = entity.Description,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime,
        PasswordHash = entity.PasswordHash
    };
} 