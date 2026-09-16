using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class ContractRepository : BaseRepository<Contract>
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {

        }

        public override Task<List<Contract>> GetCollectionAsync()
        {
            return _dbSet.Include(c=>c.Price).ToListAsync();
        }


        public Contract GetContractById(int id)
        {
            return _dbSet.Where(c => c.Id == id).Include(c => c.Price).FirstOrDefault();
        }

        public List<Contract> GetContractsWithBills()
        {
            var contractsWithBills = _dbSet.Include(c => c.Bills).ToList();
            return contractsWithBills;
        }
    }
}
