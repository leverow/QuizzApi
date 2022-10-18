using Microsoft.EntityFrameworkCore;
using quizz.Models;
using quizz.Models.Quiz;
using quizz.Repositories;
using quizz.Utils;

namespace quizz.Services;

public partial class QuizService : IQuizService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuizService> _logger;

    public QuizService(
        IUnitOfWork unitOfWork,
        ILogger<QuizService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<Result<Quiz>> CreateAsync(string title, string description, DateTime startTime, DateTime endTime, string? password = null)
    {
        if(string.IsNullOrWhiteSpace(title))
            return new("Title is invalid.");

        if(string.IsNullOrWhiteSpace(description))
            return new("Description is invalid");

        Entities.Quiz? quizEntity;
        if(!string.IsNullOrWhiteSpace(password))
            quizEntity = new Entities.Quiz(title,description,startTime,endTime) { PasswordHash = password.Sha256() };
        else
            quizEntity = new Entities.Quiz(title,description,startTime,endTime);

        try
        {
            var createdQuiz = await _unitOfWork.Quizzes.AddAsync(quizEntity);
            return new(true) { Data = ToModel(createdQuiz) };
        }
        catch(DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("Couldn't create quiz. Contact support.");
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuizService)}", e);
            throw new("Couldn't create quiz. Contact support.", e);
        }
    }

    public async ValueTask<Result<List<Quiz>>> GetAllQuizzesPaginatedAsync(int page = 1, int limit = 100)
    {
        try
        {
            var existingQuizzes = _unitOfWork.Quizzes.GetAll();
            if(existingQuizzes is null)
                return new("No quizzes found.");

            var filteredQuizzes = await existingQuizzes
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(e => ToModel(e))
                .ToListAsync();

            return new(true) { Data = filteredQuizzes };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuizService)}", e);
            throw new("Couldn't get quizzes. Contact support", e);
        }
    }

    public async ValueTask<Result<Quiz>> GetByIdAsync(ulong id)
    {
        try
        {
            var existingQuiz = await _unitOfWork.Quizzes.GetAll().FirstOrDefaultAsync(e => e.Id == id);
            if(existingQuiz is null)
                return new("Quiz with given ID not found.");

            return new(true) { Data = ToModel(existingQuiz) };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuizService)}", e);
            throw new("Couldn't get quiz. Contact support.", e);
        }
    }

    public async ValueTask<Result<Quiz>> UpdateAsync(ulong id, string title, string description, DateTime startTime, DateTime endTime, string? password = null)
    {
        var existingQuiz = _unitOfWork.Quizzes.GetById(id);
        if(existingQuiz is null)
            return new("Quiz with given ID not found.");

        existingQuiz.Title = title;
        existingQuiz.Description = description;
        existingQuiz.StartTime = startTime;
        existingQuiz.EndTime = endTime;
        
        if(!string.IsNullOrWhiteSpace(password))
            existingQuiz.PasswordHash = password.Sha256();

        try
        {
            var updatedQuiz = await _unitOfWork.Quizzes.Update(existingQuiz);
            return new(true) { Data = ToModel(updatedQuiz) };
        }
        catch(DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("Couldn't update quiz. Contact support.");
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuizService)}", e);
            throw new("Couldn't update quiz. Contact support.", e);
        }
    }

    public async ValueTask<Result<Quiz>> RemoveByIdAsync(ulong id)
    {
        try
        {
            var existingQuiz = _unitOfWork.Quizzes.GetById(id);
            if(existingQuiz is null)
                return new("Quiz with given ID not found.");

            var removedQuiz = await _unitOfWork.Quizzes.Remove(existingQuiz);
            if(removedQuiz is null)
                return new("Removing the quiz failed. Contact support.");

            return new(true) { Data = ToModel(removedQuiz) };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(QuizService)}", e);
            throw new("Couldn't remove quiz. Contact support.", e);
        }
    }

    public async ValueTask<bool> ExistsAsync(ulong id)
    {
        var quizResult = await GetByIdAsync(id);
        return quizResult.IsSuccess;
    }
}