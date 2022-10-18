namespace quizz.Models;

public class Question
{
    public ulong Id { get; set; }
    public ulong TopicId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public EQuestionType Type { get; set; }
    public uint TimeAllowed { get; set; }
}