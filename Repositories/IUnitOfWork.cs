namespace quizz.Repositories;

public interface IUnitOfWork : IDisposable
{
    ITopicRepository Topics { get; }
    IQuizRepository Quizzes { get; }
    IQuestionRepository Questions { get; }
    IMcqOptionRepository McqOptions { get; }
    ITestCaseRepository TestCases { get; }
    int Save();
}