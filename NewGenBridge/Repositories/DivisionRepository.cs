using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class DivisionRepository : BaseRepository<Division>
    {
        public DivisionRepository(ApplicationDbContext context) : base(context)
        {
             
        }
        public override Task<List<Division>> GetCollectionAsync()
        {
            return _dbSet.Include(b => b.Branches).ToListAsync();
        }

        public Task<List<Division>> GetCleanDivisionsAsync() => _dbSet.ToListAsync();
        public async Task<Division> GetByIdWithBranches(int id)
        {
            return await _dbSet.Include(b => b.Branches).FirstOrDefaultAsync(d => d.Id == id);
        }


    }
}
