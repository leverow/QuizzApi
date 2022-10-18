namespace quizz.Dtos;

public class Filter
{
    public int? Page { get; set; }
    public int? Limit { get; set; }
    public string? Topic { get; set; }
    public string? Difficulty { get; set; }
    public string? Search { get; set; }
}