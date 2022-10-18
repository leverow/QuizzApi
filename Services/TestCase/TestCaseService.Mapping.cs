namespace quizz.Services;

public partial class TestCaseService
{
    private static Models.TestCase ToModel(Entities.TestCase entity)
     => new()
     {
         Id = entity.Id,
         FileName = entity.FileName,
         QuestionId = entity.QuestionId
     };
}