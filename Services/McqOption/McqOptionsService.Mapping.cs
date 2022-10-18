

namespace quizz.Services;

public partial class McqOptionService : IMcqOptionService
{
    public Entities.McqOption ToEntity(Models.McqOption model, ulong questionId)
    => new(model.Id, questionId, model.IsTrue, model.Content);
}