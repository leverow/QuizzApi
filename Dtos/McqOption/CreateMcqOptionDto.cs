using System.ComponentModel.DataAnnotations;

namespace quizz.Dtos.McqOption;

public class CreateMcqOptionDto
{
    [Required]
    public bool IsTrue { get; set; }

    [Required, MaxLength(255)]
    public string? Content { get; set; }
}