using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class PriceRepository : BaseRepository<Price>
    {
        public PriceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
