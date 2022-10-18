namespace quizz.Entities;

public class TestCase
{
    public ulong Id { get; set; }
    public string? FileName { get; set; }
    public ulong QuestionId { get; set; }

    [Obsolete("This constructor only use ")]
    public TestCase() {}
    public TestCase(string fileName, ulong questionId)
    {
        FileName = fileName;
        QuestionId = questionId;
    }
}