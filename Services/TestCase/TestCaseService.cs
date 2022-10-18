using Microsoft.EntityFrameworkCore;
using quizz.Models;
using quizz.Repositories;

namespace quizz.Services;
public partial class TestCaseService : ITestCaseService
{
    private readonly ILogger<TestCase> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;

    public TestCaseService(
        ILogger<TestCase> logger, 
        IUnitOfWork unitOfWork, 
        IFileHelper fileHelper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
    }
    public async ValueTask<Result<TestCase>> CreateTestCasesAsync(IFormFile file, ulong questionId)
    {
        var existingQuestionId = _unitOfWork.Questions.GetById(questionId);

        if (existingQuestionId is null)
            return new("Question is not exist");

        var existingTestCase = _unitOfWork.TestCases.GetAll().Where(q => q.QuestionId == questionId);

        if (existingQuestionId is not null)
            return new("TestCase already exists");

        if (!await _fileHelper.ValidateTestCaseAsync(file))
            return new("File is invalid");

        var savedFileName = await _fileHelper.WriteTestCaseAsync(file, questionId);
        var testCaseEntity = new quizz.Entities.TestCase(savedFileName!, questionId);
       
        try
        {
            var createdTestCase = await _unitOfWork.TestCases.AddAsync(testCaseEntity);

            return new(true) { Data = ToModel(createdTestCase) };
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.LogInformation("Error occured:", dbUpdateException);
            return new("TestCase already exists.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TestCaseService)}", e);
            throw new("Couldn't create TestCase. Contact support.", e);
        }
    }
    public async ValueTask<Result<FileStream>> GetAllTestCasesAsync(string filename)
    {
        var existingFilename = _unitOfWork.TestCases.GetAll().Where(f => f.FileName == filename);

        if (existingFilename is null)
            return new("Question is not exist");

        try
        {
            var file = await _fileHelper.GetTestCaseAsync(filename);
            return new(true) { Data = file };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TestCaseService)}", e);
            throw new("Couldn't get testCase. Contact support.", e);
        }
    }
    public async ValueTask<Result<IEnumerable<string>>> GetTestCaseByQuestionIdAsync(FileStream stream)
    {
        if (stream is null)
            return new("TestCase is not exist");

        try
        {
            var file = FileHelper.GetFilesInZip(stream);

            return new(true) { Data = file };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occured at {nameof(TestCaseService)}", e);
            throw new("Couldn't get TestCase. Contact support.", e);
        }
    }
}