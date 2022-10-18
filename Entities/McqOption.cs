namespace quizz.Entities;

public class McqOption
{
    public ulong Id { get; set; }
    public ulong QuestionId { get; set; }
    public bool IsTrue { get; set; }
    public string? Content { get; set; }

    [Obsolete("This constructor only be used by EF CORE")]
    public McqOption() { }

    public McqOption(ulong id, ulong questionId, bool isTrue, string? content)
    {
        Id = id;
        QuestionId = questionId;
        IsTrue = isTrue;
        Content = content;
    }
}