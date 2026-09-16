using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class BranchRepository : BaseRepository<Branch>
    {
        public BranchRepository(ApplicationDbContext context) : base(context)
        {
        }


        public Branch GetFullBranchAsync(int id)
        {
            //get branch with groups
            //todo: Just in case, check if the branch exists
            return _dbSet.Where(b => b.Id == id)
                         .Include(b => b.Groups)
                         .ThenInclude(g => g.Users)
                         .ElementAt(0);
        }

        public override Task<List<Branch>> GetCollectionAsync()
        {
            //get collection of branches with groups
            return _dbSet.Include(b => b.Groups).ToListAsync();
        }

        public Task<List<Branch>> GetFullCollectionAsync()
        {
            return _dbSet.Include(b => b.Groups).ThenInclude(g => g.Users).ToListAsync();
        }

        public Task<List<Branch>> GetCollectionAsync(int divisionId)
        {
            return _dbSet.Where(b => b.DivisionId == divisionId).Include(b => b.Groups).ToListAsync();
        }

        public Task<List<Branch>> GetFullCollectionAsync(int divisionId)
        {
            return _dbSet.Where(b => b.DivisionId == divisionId)
                         .Include(b => b.Groups)
                         .ThenInclude(g => g.Users)
                         .ToListAsync();
        }

        public Task<List<Branch>> GetFullCollectionAsync(int[] branchIds)
        {
            return _dbSet.Where(b => branchIds.Contains(b.Id))
                         .Include(b => b.Groups)
                         .ThenInclude(g => g.Users)
                         .ToListAsync();
        }
    }
}
