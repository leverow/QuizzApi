namespace quizz.Entities;

public class Topic : EntityBase
{
    public string? Name { get; set; }
    public string? NameHash { get; set; }    
    public string? Description { get; set; }
    public ETopicDifficulty Difficulty { get; set; }

    public virtual ICollection<Question>? Questions { get; set; }

    [Obsolete("Used only for entity binding.")]
    public Topic() { }

    public Topic(
        string name,
        string description,
        ETopicDifficulty difficulty)
    {
        Name = name;
        Description = description;
        Difficulty = difficulty;
    }
}