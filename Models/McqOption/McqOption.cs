namespace quizz.Models;

public class McqOption
{
    public ulong Id { get; set; }
    public ulong QuestionId { get; set; }
    public bool IsTrue { get; set; }
    public string? Content { get; set; }
}
 
