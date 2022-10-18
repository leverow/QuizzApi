namespace quizz.Models;

public class TestCase
{
    public ulong Id { get; set; }
    public string? FileName { get; set; }
    public ulong QuestionId { get; set; }
}