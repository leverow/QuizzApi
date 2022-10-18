using Microsoft.AspNetCore.Mvc;
using quizz.Dtos;
using quizz.Dtos.Quiz;
using quizz.Services;

namespace quizz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly ILogger<QuizzesController> _logger;
    private readonly IQuizService _quizService;

    public QuizzesController(
        ILogger<QuizzesController> logger,
        IQuizService quizService
    )
    {
        _logger = logger;
        _quizService = quizService;
    }
    
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Quiz>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetQuizzes([FromQuery]Pagination pagination)
    {
        try
        {
            var quizzesResult = await _quizService.GetAllQuizzesPaginatedAsync(pagination.Page, pagination.Limit);
            if(!quizzesResult.IsSuccess)
                return NotFound(new { ErrorMessage = quizzesResult.ErrorMessage });
        
            return Ok(quizzesResult?.Data?.Select(ToDto));

        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet]
    [Route("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Quiz))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> GetQuiz([FromRoute]ulong id)
    {
        try
        {
            if(id < 1)
                return BadRequest(new { ErrorMessage = "ID is wrong."});

            var quizResult = await _quizService.GetByIdAsync(id);

            if(!quizResult.IsSuccess || quizResult.Data is null)
                return NotFound(new { ErrorMessage = quizResult.ErrorMessage });

            return Ok(ToDto(quizResult.Data));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Quiz))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> PostQuiz(CreateQuizDto model)
    {
        try
        {
            if(!ModelState.IsValid) 
                return BadRequest(model);

            var createQuizResult = await _quizService.CreateAsync(model.Title!, model.Description!, model.StartTime, model.EndTime, model.Password);
            if(!createQuizResult.IsSuccess)
                return BadRequest(new { ErrorMessage = createQuizResult.ErrorMessage });
            
            return CreatedAtAction(nameof(GetQuiz), new { Id = createQuizResult?.Data?.Id }, ToDto(createQuizResult?.Data!));
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
    public async Task<IActionResult> DeleteQuiz(ulong id)
    {
        try
        {
            var removeQuizResult = await _quizService.RemoveByIdAsync(id);

            if(!removeQuizResult.IsSuccess || removeQuizResult.Data is null)
                return NotFound(new { ErrorMessage = removeQuizResult.ErrorMessage });

            return Ok();
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Quiz))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Error))]
    public async Task<IActionResult> UpdateQuiz([FromRoute]ulong id, [FromBody]UpdateQuizDto model)
    {
        try
        {
            if(!ModelState.IsValid)
                return BadRequest(model);
            
            if(!await _quizService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Quiz with given ID not found." });

            var updateQuizResult = await _quizService.UpdateAsync(id, model.Title!, model.Description!, model.StartTime, model.EndTime, model.Password);
            if(!updateQuizResult.IsSuccess)
                return BadRequest(new { ErrorMessage = updateQuizResult.ErrorMessage });
            
            return Ok(ToDto(updateQuizResult?.Data!));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    private Quiz ToDto(Models.Quiz.Quiz entity)
    {
        return new Quiz()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}