using quizz.Data;
using quizz.Entities;

namespace quizz.Repositories;

public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
{
    public QuizRepository(ApplicationDbContext context)
        : base(context) { }
}