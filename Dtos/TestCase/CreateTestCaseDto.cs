using System.ComponentModel.DataAnnotations;

namespace quizz.Dtos;

public class CreateTestCaseDto
{
    public IFormFile? File { get; set; }
}