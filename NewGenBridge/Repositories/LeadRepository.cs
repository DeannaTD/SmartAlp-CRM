using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class LeadRepository : BaseRepository<Lead>
    {
        public LeadRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
