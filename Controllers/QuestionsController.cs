using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using quizz.Dtos;
using quizz.Dtos.McqOption;
using quizz.Dtos.Question;
using quizz.Dtos.Quiz;
using quizz.Services;

namespace quizz.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class QuestionsController : ControllerBase
{
    private readonly ILogger<QuestionsController> _logger;
    private readonly IQuestionService _questionService;
    private readonly IMcqOptionService _mcqOptionService;

    public QuestionsController(
        ILogger<QuestionsController> logger,
        IQuestionService questionService,
        IMcqOptionService mcqOptionService)
    {
        _logger = logger;
        _questionService = questionService;
        _mcqOptionService = mcqOptionService;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Quiz>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetQuestions([FromQuery] Filter filter)
    {
        try
        {
            Enum.TryParse(filter.Difficulty, true, out Models.Topic.ETopicDifficulty difficulty);

            var questionsResult = await _questionService.GetAllQuestionsAsync(
                page: filter.Page ?? 1,
                limit: filter.Limit ?? 100,
                topic: filter.Topic,
                search: filter.Search,
                difficulty: difficulty
            );

            if (!questionsResult.IsSuccess)
                return NotFound(new { ErrorMessage = questionsResult.ErrorMessage });

            return Ok(questionsResult?.Data?.Select(ToDto));

        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Question))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetQuestion([FromRoute] ulong id)
    {
        try
        {
            if (id < 1)
                return BadRequest(new { ErrorMessage = "ID is wrong." });

            var questionResult = await _questionService.GetByIdAsync(id);

            if (!questionResult.IsSuccess || questionResult.Data is null)
                return NotFound(new { ErrorMessage = questionResult.ErrorMessage });

            return Ok(ToDto(questionResult.Data));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet("search")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Question))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetQuestionByTitle([FromQuery] string? title)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(new { ErrorMessage = "Title is required to search by." });

            var questionResult = await _questionService.FindByTitleAsync(title);

            if (!questionResult.IsSuccess || questionResult.Data is null)
                return NotFound(new { ErrorMessage = questionResult.ErrorMessage });

            return Ok(ToDto(questionResult.Data));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Quiz))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> PostQuestion([FromForm] CreateQuestionDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        try
        {
            var createQuestionResult = await _questionService.CreateAsync(model.Title!, model.Description!, ToModel(model.Type), model.TimeAllowed, model.TopicId);
            if (!createQuestionResult.IsSuccess)
                return BadRequest(new { ErrorMessage = createQuestionResult.ErrorMessage });

            return CreatedAtAction(nameof(GetQuestion), new { Id = createQuestionResult?.Data?.Id }, ToDto(createQuestionResult?.Data!));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> DeleteQuestion(ulong id)
    {
        try
        {
            var removeQuestionResult = await _questionService.RemoveByIdAsync(id);

            if (!removeQuestionResult.IsSuccess || removeQuestionResult.Data is null)
                return NotFound(new { ErrorMessage = removeQuestionResult.ErrorMessage });

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Quiz))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> UpdateQuestion([FromRoute] ulong id, [FromBody] UpdateQuestionDto model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(model);

            if (!await _questionService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Question with given ID not found." });

            var updateQuestionResult = await _questionService.UpdateAsync(id, model.Title!, model.Description!, ToModel(model.Type), model.TimeAllowed, model.TopicId);
            if (!updateQuestionResult.IsSuccess)
                return BadRequest(new { ErrorMessage = updateQuestionResult.ErrorMessage });

            return Ok(ToDto(updateQuestionResult?.Data!));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }
    
    private Question ToDto(Models.Question model)
    => new()
    {
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        Type = ToDto(model.Type),
        TimeAllowed = model.TimeAllowed,
        TopicId = model.TopicId
    };

    private EQuestionType ToDto(Models.EQuestionType model)
    => model switch
    {
        Models.EQuestionType.MultipleChoice => EQuestionType.MultipleChoice,
        _ => EQuestionType.Algorithmic,
    };

    private Models.EQuestionType ToModel(EQuestionType dto)
    => dto switch
    {
        EQuestionType.MultipleChoice => Models.EQuestionType.MultipleChoice,
        _ => Models.EQuestionType.Algorithmic,
    };

    private McqOption ToDto(Models.McqOption model)
    => new()
    {
        Id = model.Id,
        Content = model.Content
    };
}