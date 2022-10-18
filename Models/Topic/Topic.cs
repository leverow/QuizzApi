namespace quizz.Models.Topic;

public class Topic
{
    public ulong Id { get; set; }
    public string? Name { get; set; }   
    public string? Description { get; set; }
    public ETopicDifficulty Difficulty { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }  
}