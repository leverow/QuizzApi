using quizz.Models;

namespace quizz.Services;

public partial class McqOptionService
{
    public static McqOption ToModel(Entities.McqOption entity)
    => new()
    {
        Id = entity.Id,
        QuestionId = entity.QuestionId,
        IsTrue = entity.IsTrue,
        Content = entity.Content  
    };
}