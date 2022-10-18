namespace quizz.Entities;

public class Quiz : EntityBase
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? PasswordHash { get; set; }

    [Obsolete("This constructor only be used by EF CORE")]
    public Quiz(){}

    public Quiz(
        string title,
        string description,
        DateTime startTime,
        DateTime endTime
    )
    {
        Title = title;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
    }
}