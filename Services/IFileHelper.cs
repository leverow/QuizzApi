namespace quizz.Services;

public interface IFileHelper
{
    ValueTask<bool> ValidateTestCaseAsync(IFormFile file);
    ValueTask<string?> WriteTestCaseAsync(IFormFile file, ulong questionId);
    ValueTask<FileStream?> GetTestCaseAsync(string filename);
}