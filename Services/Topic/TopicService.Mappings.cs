using quizz.Models.Topic;

namespace quizz.Services;

public partial class TopicService
{
    public static Topic ToModel(Entities.Topic entity)
    => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Name = entity.Name,
        Description = entity.Description,
        Difficulty = ToModel(entity.Difficulty)
    };

    public static ETopicDifficulty ToModel(Entities.ETopicDifficulty entity)
    => entity switch
    {
        Entities.ETopicDifficulty.Beginner => ETopicDifficulty.Beginner,
        Entities.ETopicDifficulty.Intermediate => ETopicDifficulty.Intermediate,
        _ => ETopicDifficulty.Advanced,
    };

    public static Entities.ETopicDifficulty ToEntity(ETopicDifficulty model)
    => model switch
    {
        ETopicDifficulty.Beginner => Entities.ETopicDifficulty.Beginner,
        ETopicDifficulty.Intermediate => Entities.ETopicDifficulty.Intermediate,
        _ => Entities.ETopicDifficulty.Advanced,
    };
}