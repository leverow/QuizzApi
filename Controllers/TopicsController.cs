using Microsoft.AspNetCore.Mvc;
using quizz.Dtos;
using quizz.Dtos.Topic;
using quizz.Services;

namespace quizz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{
    private readonly ILogger<TopicsController> _logger;
    private readonly ITopicService _topicService;

    public TopicsController(
        ITopicService topicService,
        ILogger<TopicsController> logger)
    {
        _logger = logger;
        _topicService = topicService;
    }

    [HttpGet("search")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Topic))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetTopicByName([FromQuery]string? name)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(name))
                return BadRequest(new { ErrorMessage = "Name is required to search by."});

            var topicResult = await _topicService.FindByNameAsync(name);

            if(!topicResult.IsSuccess || topicResult.Data is null)
                return NotFound(new { ErrorMessage = topicResult.ErrorMessage });

            return Ok(ToDto(topicResult.Data));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Topic>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetTopics([FromQuery]Pagination pagination)
    {
        try
        {
            var topicsResult = await _topicService.GetAllPaginatedTopicsAsync(pagination.Page, pagination.Limit);
            if(!topicsResult.IsSuccess)
                return NotFound(new { ErrorMessage = topicsResult.ErrorMessage });
        
            return Ok(topicsResult?.Data?.Select(ToDto));

        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Topic))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetTopic([FromRoute]ulong id)
    {
        try
        {
            if(id < 1)
                return BadRequest(new { ErrorMessage = "ID is wrong."});

            var topicResult = await _topicService.GetByIdAsync(id);

            if(!topicResult.IsSuccess || topicResult.Data is null)
                return NotFound(new { ErrorMessage = topicResult.ErrorMessage });

            return Ok(ToDto(topicResult.Data));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }
    

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Topic))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> PostTopic(CreateTopicDto model)
    {
        try
        {
            if(!ModelState.IsValid) 
                return BadRequest(model);
            
            var createTopicResult = await _topicService.CreateAsync(model.Name!, model.Description!, ToModel(model.Difficulty));
            if(!createTopicResult.IsSuccess)
                return BadRequest(new { ErrorMessage = createTopicResult.ErrorMessage });
            
            return CreatedAtAction(nameof(GetTopic), new { Id = createTopicResult?.Data?.Id }, ToDto(createTopicResult?.Data!));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> DeleteTopic(ulong id)
    {
        try
        {
            var removeTopicResult = await _topicService.RemoveByIdAsync(id);

            if(!removeTopicResult.IsSuccess || removeTopicResult.Data is null)
                return NotFound(new { ErrorMessage = removeTopicResult.ErrorMessage });

            return Ok();
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Topic))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> UpdateTopic([FromRoute]ulong id, [FromBody]UdpateTopicDto model)
    {
        try
        {
            if(!ModelState.IsValid)
                return BadRequest(model);
            
            if(!await _topicService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Topic with given ID not found." });

            var updateTopicResult = await _topicService.UpdateAsync(id, model.Name!, model.Description!, ToModel(model.Difficulty));
            if(!updateTopicResult.IsSuccess)
                return BadRequest(new { ErrorMessage = updateTopicResult.ErrorMessage });
            
            return Ok(ToDto(updateTopicResult?.Data!));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    private Models.Topic.ETopicDifficulty ToModel(ETopicDifficulty difficulty)
    => difficulty switch
    {
        ETopicDifficulty.Beginner => Models.Topic.ETopicDifficulty.Beginner,
        ETopicDifficulty.Intermidiate => Models.Topic.ETopicDifficulty.Intermediate,
        _ => Models.Topic.ETopicDifficulty.Advanced,
    };

    private Topic ToDto(Models.Topic.Topic entity)
    {
        return new Topic()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Difficulty = entity.Difficulty.ToString(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}