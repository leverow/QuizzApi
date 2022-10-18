using quizz.Data;
using quizz.Entities;

namespace quizz.Repositories;

public class TestCaseRepository : GenericRepository<TestCase>, ITestCaseRepository
{
    public TestCaseRepository(ApplicationDbContext context) : base(context) { }
}