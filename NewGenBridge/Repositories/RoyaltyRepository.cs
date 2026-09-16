using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class RoyaltyRepository : BaseRepository<Royalty>
    {
        public RoyaltyRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
