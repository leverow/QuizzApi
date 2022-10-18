using Microsoft.AspNetCore.Mvc;
using quizz.Dtos.McqOption;

namespace quizz.Controllers;

public partial class QuestionsController
{
    [HttpPost("{id}/options")]
    public async Task<IActionResult> PostMcqOptionAsync([FromBody]List<CreateMcqOptionDto> dtos,[FromRoute]ulong id)
    {
        try
        {
            if (id < 1)
                return BadRequest(new { ErrorMessage = "Question ID is wrong." });

            if (!await _questionService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Question with given ID not found." });

            var optionModels = dtos.Select(ToModel).ToList();
            var createdOptionResult = await _mcqOptionService.CreateOptionsAsync(optionModels, id);

            if (!createdOptionResult.IsSuccess)
                return BadRequest(new { ErrorMessage = createdOptionResult.ErrorMessage });

            return CreatedAtAction(nameof(GetAllOptionsAsync), new { Id = createdOptionResult?.Data?.QuestionId }, ModelToDto(createdOptionResult?.Data!));
        }
        catch(Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpGet("{id}/options")]
    public async Task<IActionResult> GetAllOptionsAsync([FromRoute]ulong id)
    {
        try
        {
            if (id < 1)
                return BadRequest(new { ErrorMessage = "Question ID is wrong." });

            if (!await _questionService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Question with given ID not found." });

            var optionsResult = await _mcqOptionService.GetAllOptionsByQuestionIdAsync(id);

            if (!optionsResult.IsSuccess)
                return NotFound(new { ErrorMessage = optionsResult.ErrorMessage });

            return Ok(optionsResult?.Data?.Select(ModelToDto));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    [HttpDelete("{id}/options/clear")]
    public async Task<IActionResult> DeleteOptionsByQuestionIdAsync(ulong id)
    {
        try
        {
            if (id < 1)
                return BadRequest(new { ErrorMessage = "Question ID is wrong." });
            
            if (!await _questionService.ExistsAsync(id))
                return NotFound(new { ErrorMessage = "Question with given ID not found." });

            var removedOptionResult = await _mcqOptionService.RemoveOptionsByQuestionIdAsync(id);

            if (!removedOptionResult.IsSuccess || removedOptionResult.Data is null)
                return NotFound(new { ErrorMessage = removedOptionResult.ErrorMessage });

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { ErrorMessage = e.Message });
        }
    }

    private McqOption ModelToDto(Models.McqOption model)
    => new()
    {
        Id = model.Id,
        Content = model.Content
    };
    private Models.McqOption ToModel(CreateMcqOptionDto dto)
    => new()
    {
        IsTrue = dto.IsTrue,
        Content = dto.Content
    };
}
