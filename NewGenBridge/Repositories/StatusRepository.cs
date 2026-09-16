using NewGenBridge.Data;
using NewGenBridge.Models;
namespace NewGenBridge.Repositories
{
    public class StatusRepository : BaseRepository<Status>
    {
        public StatusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
