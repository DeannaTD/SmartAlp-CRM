using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class BillRepository : BaseRepository<Bill>
    {
        public BillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Bill>> GetFullBillsAsync()
        {
            //get branch with groups
            //todo: Just in case, check if the branch exists
            var result = await _dbSet.Include(b => b.Contract).ToListAsync();
            return result;
        }
    }
}
