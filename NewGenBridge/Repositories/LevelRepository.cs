using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class LevelRepository : BaseRepository<Level>
    {
        public LevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<List<Level>> GetCollectionAsync()
        {
            return _dbSet.Include(l => l.Lessons).ToListAsync();
        }

        public List<Level> GetCleanCollection()
        {
            return _dbSet.ToList();
        }
    }
}
