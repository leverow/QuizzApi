namespace quizz.Entities;

public abstract class EntityBase
{
    public ulong Id { get; set; }  
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}