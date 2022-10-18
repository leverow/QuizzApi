using System.ComponentModel.DataAnnotations;

namespace quizz.Dtos.Quiz;

public class UpdateQuizDto
{
    [Required, MaxLength(255)]
    public string? Title { get; set; }
    [Required, MaxLength(int.MaxValue)]
    public string? Description { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [Range(3,10)]
    public string? Password { get; set; }
}