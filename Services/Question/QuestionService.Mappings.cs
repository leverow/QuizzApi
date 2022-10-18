using System.Linq.Expressions;
using quizz.Models;

namespace quizz.Services;

public partial class QuestionService
{
    public static Question ToModel(Entities.Question entity)
    => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        TopicId = entity.TopicId,
        TimeAllowed = entity.TimeAllowed,
        Type = ToModel(entity.Type)
    };

    public static EQuestionType ToModel(Entities.EQuestionType entity)
    => entity switch
    {
        Entities.EQuestionType.Algorithmic => EQuestionType.Algorithmic,
        _ => EQuestionType.MultipleChoice,
    };

    public static Entities.EQuestionType ToEntity(Models.EQuestionType model)
    => model switch
    {
        Models.EQuestionType.Algorithmic => Entities.EQuestionType.Algorithmic,
        _ => Entities.EQuestionType.MultipleChoice,
    };
    private static Entities.ETopicDifficulty ToDifficultyEntity(Models.Topic.ETopicDifficulty difficulty)
    => difficulty switch
    {
        Models.Topic.ETopicDifficulty.Beginner => Entities.ETopicDifficulty.Beginner,
        Models.Topic.ETopicDifficulty.Intermediate => Entities.ETopicDifficulty.Intermediate,
        _ => Entities.ETopicDifficulty.Advanced
    };

    private static Expression<Func<Entities.Question, bool>> QuestionFilter(string search, string topic, Models.Topic.ETopicDifficulty difficulty)
    {
        Expression<Func<Entities.Question, bool>> filter = question
        => question.Title!.Contains(search)
        && question.Topic != null
        && question.Topic.Name!.ToLower().Contains(topic)
        && (int)question.Topic.Difficulty == (int)ToDifficultyEntity(difficulty);

        return filter;
    }
}