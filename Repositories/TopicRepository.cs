using quizz.Data;
using quizz.Entities;

namespace quizz.Repositories;

public class TopicRepository : GenericRepository<Topic>, ITopicRepository
{
    public TopicRepository(ApplicationDbContext context)
        : base(context) { }
}