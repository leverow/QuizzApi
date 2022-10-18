using quizz.Data;

namespace quizz.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public ITopicRepository Topics { get; }
    public IQuizRepository Quizzes { get; }
    public IQuestionRepository Questions { get; }
    public IMcqOptionRepository McqOptions { get; set; }
    public ITestCaseRepository TestCases { get; set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Topics = new TopicRepository(context);
        Quizzes = new QuizRepository(context);
        Questions = new QuestionRepository(context);
        McqOptions = new McqOptionRepository(context);
        TestCases = new TestCaseRepository(context);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public int Save()
        => _context.SaveChanges();
}