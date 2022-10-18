namespace quizz.Entities;

public class Question : EntityBase
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public EQuestionType Type { get; set; }
    public uint TimeAllowed { get; set; }
    public ulong TopicId { get; set; }
    public virtual Topic? Topic { get; set; }

    [Obsolete("This constructor only be used by EF CORE")]
    public Question(){}

    public Question(
        ulong topicId,
        string? title,
        string? description,
        EQuestionType type,
        uint timeAllowed)
    {
        TopicId = topicId;
        Title = title;
        Description = description;
        Type = type;
        TimeAllowed = timeAllowed;
    }
}