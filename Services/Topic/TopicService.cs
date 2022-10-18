using Microsoft.EntityFrameworkCore;
using quizz.Models;
using quizz.Models.Topic;
using quizz.Repositories;
using quizz.Utils;

namespace quizz.Services;

public partial class TopicService : ITopicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TopicService> _logger;

    public TopicService(IUnitOfWork unitOfWork, ILogger<TopicService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<bool> ExistsAsync(ulong id)
    {
        var topicResult = await GetByIdAsync(id);
        return topicResult.IsSuccess;
    }

    public async ValueTask<Result<Topic>> CreateAsync(string name, string description, ETopicDifficulty difficulty)
    {
        if(string.IsNullOrWhiteSpace(name))
            return new("Name is invalid.");
        
        if(string.IsNullOrWhiteSpace(description))
            return new("Description is invalid");
        
        var topicEntity = new Entities.Topic(name, description, ToEntity(difficulty));

        try
        {
            var createdTopic = await _unitOfWork.Topics.AddAsync(topicEntity);
            return new(true) { Data = ToModel(createdTopic) }; 
        }
        catch(DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("Topic already exists.");
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't create topic. Contact support.", e);
        }
    }

    public async ValueTask<Result<Topic>> FindByNameAsync(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            return new("Name is invalid.");
        try
        {
            var nameHash = name.ToLower().Sha256();

            var existingTopic = await _unitOfWork.Topics.GetAll().FirstOrDefaultAsync(t => t.NameHash == nameHash);
            if(existingTopic is null)
                return new("No topic found for given name.");

            return new(true) { Data = ToModel(existingTopic) };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't search topic. Contact support.", e);
        }
    }

    public async ValueTask<Result<List<Topic>>> GetAllPaginatedTopicsAsync(int page, int limit)
    {
        try
        {
            var existingTopics = _unitOfWork.Topics.GetAll();
            if(existingTopics is null)
                return new("No topics found. Contact support.");

            var filteredTopics = await existingTopics
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(e => ToModel(e))
                .ToListAsync();
            
            return new(true) { Data = filteredTopics };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't get topics. Contact support.", e);
        }
    }

    public async ValueTask<Result<Topic>> GetByIdAsync(ulong id)
    {
        try
        {
            var existingTopic = await _unitOfWork.Topics.GetAll().FirstOrDefaultAsync(t => t.Id == id);
            if(existingTopic is null)
                return new("Topic with given ID not found.");

            return new(true) { Data = ToModel(existingTopic) };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't get topic. Contact support.", e);
        }
    }

    public async ValueTask<Result<Topic>> RemoveByIdAsync(ulong id)
    {
        try
        {
            var existingTopic = _unitOfWork.Topics.GetById(id);
            if(existingTopic is null)
                return new("Topic with given ID not found.");

            var removedTopic = await _unitOfWork.Topics.Remove(existingTopic);
            if(removedTopic is null)
                return new("Removing the topic failed. Contact support.");

            return new(true) { Data = ToModel(removedTopic) };
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't remove topic. Contact support.", e);
        }
    }

    public async ValueTask<Result<Topic>> UpdateAsync(ulong id, string name, string description, ETopicDifficulty difficulty)
    {
        // TODO: Validate name, description

        var existingTopic = _unitOfWork.Topics.GetById(id);
        if(existingTopic is null)
            return new("Topic with given ID not found.");
        
        existingTopic.Name = name;
        existingTopic.Description = description;
        existingTopic.Difficulty = ToEntity(difficulty);

        try
        {
            var updatedTopic = await _unitOfWork.Topics.Update(existingTopic);
            return new(true) { Data = ToModel(updatedTopic) };
        }
        catch(DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("Topic name already exists.");
        }
        catch(Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TopicService)}", e);
            throw new("Couldn't update topic. Contact support.", e);
        }
    }
}