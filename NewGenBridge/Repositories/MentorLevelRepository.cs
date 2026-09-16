using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class MentorLevelRepository : BaseRepository<MentorLevel>
    {
        public MentorLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<MentorLevel> GetLevelsByMentor(string mentorId)
        {
            return _dbSet.Where(ml => ml.MentorId == mentorId).ToList();
        }
    }
}
