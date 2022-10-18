namespace quizz.Services;

public interface ICompilerService
{
    ValueTask<string?> RunAsync(string source, string? input);
}