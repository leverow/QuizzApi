using System.ComponentModel.DataAnnotations;
using quizz.Dtos.Question;

namespace quizz.Dtos.Quiz;

public class CreateQuestionDto
{
    [Required, MaxLength(255)]
    public string? Title { get; set; }
    
    [Required, MaxLength(int.MaxValue)]
    public string? Description { get; set; }
    
    [Required]
    public EQuestionType Type { get; set; }
    
    [Required]
    public uint TimeAllowed { get; set; }
    
    [Required]
    public ulong TopicId { get; set; }
}