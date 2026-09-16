using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class KitRequestRepository : BaseRepository<KitRequest>
    {
        public KitRequestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
