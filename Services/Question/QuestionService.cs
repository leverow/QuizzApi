using Microsoft.EntityFrameworkCore;
using quizz.Models;
using quizz.Repositories;

namespace quizz.Services;

public partial class QuestionService : IQuestionService
{
    private readonly ILogger<QuestionService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public QuestionService(ILogger<QuestionService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Question>> CreateAsync(string title, string description, EQuestionType type, uint timeAllowed, ulong topicId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new("Title is invalid");

        if (string.IsNullOrWhiteSpace(description))
            return new("description is invalid");

        var existingTopicId = await _unitOfWork.Topics.GetAll().FirstOrDefaultAsync(q => q.Id == topicId);

        if (existingTopicId is null)
            return new("Topic with given Id Not Found.");

        var createdEntity = new quizz.Entities.Question(topicId, title, description, ToEntity(type), timeAllowed);

        try
        {
            var createdQuestion = await _unitOfWork.Questions.AddAsync(createdEntity);
            return new(true) { Data = ToModel(createdEntity) };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't create question. Contact support.", e);
        }
    }

    public async ValueTask<bool> ExistsAsync(ulong id)
    {
            var topicResult = await GetByIdAsync(id);
            return topicResult.IsSuccess;
    }

    public async ValueTask<Result<List<Question>>> GetAllQuestionsAsync(int page = 1, int limit = 100, string? topic = null, string? search = "", Models.Topic.ETopicDifficulty difficulty = Models.Topic.ETopicDifficulty.Beginner)
    {
        try
        {

            var filter = QuestionFilter(search ?? string.Empty, topic?.ToLower() ?? string.Empty, difficulty);

            var existingQuestions = _unitOfWork.Questions.GetAll()
                .Include(q => q.Topic)
                .Where(filter)
                .Skip((page - 1) * limit)
                .Take(limit);

            if (existingQuestions is null)
                return new("No questions found. contact support.");

            var questions = await existingQuestions.Select(q => ToModel(q)).ToListAsync();

            return new(true) { Data = questions };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't get questions. Contact support.", e);
        }
    }
    public async ValueTask<Result<Question>> FindByTitleAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new("Title is invalid.");
        try
        {
            var existingQuestion = await _unitOfWork.Questions.GetAll().FirstOrDefaultAsync(t => t.Title == title);

            if (existingQuestion is null)
                return new("No topic found for given name.");

            return new(true) { Data = ToModel(existingQuestion) };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't search question. Contact support.", e);
        }
    }

    public async ValueTask<Result<Question>> GetByIdAsync(ulong id)
    {
        try
        {
            var existingQuestion = await _unitOfWork.Questions.GetAll().FirstOrDefaultAsync(q => q.Id == id);

            if (existingQuestion is null)
                return new("Question with given Id Not Found.");

            return new(true) { Data = ToModel(existingQuestion) };

        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't get questions. Contact support.", e);
        }
    }

    public async ValueTask<Result<Question>> RemoveByIdAsync(ulong id)
    {
        try
        {
            var existingQuestion = _unitOfWork.Questions.GetById(id);

            if (existingQuestion is null)
                return new("Question with given Id Not Found.");

            var removedQuestion = await _unitOfWork.Questions.Remove(existingQuestion);

            return new(true) { Data = ToModel(removedQuestion) };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't get questions. Contact support.", e);
        }
    }

    public async ValueTask<Result<Question>> UpdateAsync(ulong id, string title, string description, EQuestionType type, uint TimeAllowed, ulong topicId)
    {
        var existingQuestion = _unitOfWork.Questions.GetById(id);

        if (existingQuestion is null)
            return new("Question with given ID not found.");

        var existingTopicId = await _unitOfWork.Topics.GetAll().FirstOrDefaultAsync(q => q.Id == topicId);

        if (existingTopicId is null)
            return new("Topic with given Id Not Found.");

        existingQuestion.Title = title;
        existingQuestion.Description = description;
        existingQuestion.TopicId = topicId;
        existingQuestion.TimeAllowed = TimeAllowed;
        existingQuestion.Type = ToEntity(type);

        try
        {
            var updatedQuestion = await _unitOfWork.Questions.Update(existingQuestion);

            return new(true) { Data = ToModel(updatedQuestion) };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuestionService)}", e);
            throw new("Couldn't update question. Contact support.", e);
        }
    }
}