using Microsoft.EntityFrameworkCore;
using quizz.Models;
using quizz.Repositories;

namespace quizz.Services;

public partial class McqOptionService : IMcqOptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<McqOptionService> _logger;

    public McqOptionService(IUnitOfWork unitOfWork, ILogger<McqOptionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Result<McqOption>> CreateOptionsAsync(List<McqOption> models, ulong questionId)
    {
        if (models.Count < 1)
            return new("Options is invalid");

        var question = _unitOfWork.Questions.GetAll().FirstOrDefault(q => q.Id == questionId);

        if (question is null) return new("Question with given Id not found.");

        if (question?.Type == Entities.EQuestionType.Algorithmic)
            return new("Question type is algorithmic");

        var hasOptionQuestion = await _unitOfWork.McqOptions.GetAll().FirstOrDefaultAsync(b => b.QuestionId == questionId);
        if (hasOptionQuestion is not null) return new("Options are already exist");
        
        try
        {
            var entityOptions = models.Select(m => ToEntity(m, questionId)).ToList();
            await _unitOfWork.McqOptions.AddRange(entityOptions);
            return new(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(McqOptionService)}", e);
            throw new("Couldn't create mcqoption.", e);
        }
    }

    public async ValueTask<Result<List<McqOption>>> GetAllOptionsByQuestionIdAsync(ulong questionId)
    {
        try
        {
            var existingMcqOption = _unitOfWork.McqOptions.GetAll()
                .Where(b => b.QuestionId == questionId);

            if (existingMcqOption is null)
                return new("No mcqOptions found. contact support.");

            var modelMcqOption = await existingMcqOption.Select(q => ModelToModel(q)).ToListAsync();

            return new(true) { Data = modelMcqOption };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(McqOptionService)}", e);
            throw new("Couldn't get mcqOption. Contact support.", e);
        }
    }

    public async ValueTask<Result<McqOption>> RemoveOptionsByQuestionIdAsync(ulong questionId)
    {
        try
        {
            var existingMcqOption = _unitOfWork.McqOptions.GetAll()
                 .Where(b => b.QuestionId == questionId);

            if (existingMcqOption is null)
                return new("No mcqOptions found. contact support.");

            await _unitOfWork.McqOptions.RemoveRange(existingMcqOption);

            return new(true);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(McqOptionService)}", e);
            throw new("Couldn't Delete mcqoption. Contact support.", e);
        }
    }

    private McqOption ModelToModel(Entities.McqOption entity)
        => new()
        {
            Id = entity.Id,
            QuestionId = entity.QuestionId,
            IsTrue = entity.IsTrue,
            Content = entity.Content
        };
}