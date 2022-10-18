using System.ComponentModel.DataAnnotations;

namespace quizz.Dtos.Quiz;

public class CreateQuizDto
{
    [Required, MaxLength(255)]
    public string? Title { get; set; }
    
    [Required, MaxLength(int.MaxValue)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [MinLength(3), MaxLength(10)]
    public string? Password { get; set; }
}