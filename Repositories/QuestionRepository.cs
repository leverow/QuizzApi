using quizz.Data;
using quizz.Entities;

namespace quizz.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context)
        : base(context) { }
}