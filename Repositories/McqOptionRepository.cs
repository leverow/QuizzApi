using quizz.Data;
using quizz.Entities;

namespace quizz.Repositories;

public class McqOptionRepository : GenericRepository<McqOption>, IMcqOptionRepository
{
    public McqOptionRepository(ApplicationDbContext context)
        : base(context) { }
}