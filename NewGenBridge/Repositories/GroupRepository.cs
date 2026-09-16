using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class GroupRepository : BaseRepository<Group>
    {
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Group> GetByIdAsync(int groupId)
        {
            return await _dbSet.Include(g => g.Users).FirstAsync(g => g.Id == groupId);
        }

        public IQueryable<Group> GetByBranchId(int branchId)
        {
            return _dbSet.Where(group => group.BranchId == branchId);
        }

    }
}
