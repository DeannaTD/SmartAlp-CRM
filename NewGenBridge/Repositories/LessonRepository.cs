using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class LessonRepository : BaseRepository<Lesson>
    {
        public LessonRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
