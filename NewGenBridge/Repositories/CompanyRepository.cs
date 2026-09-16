using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class CompanyRepository : BaseRepository<Company>
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Task<List<Company>> GetCollectionAsync()
        {
            return _dbSet.Include(c => c.Agreements).ToListAsync();
        }
    }
}
