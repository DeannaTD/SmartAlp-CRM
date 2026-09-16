using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class UserAccessRepository : BaseRepository<UserAccess>
    {
        public UserAccessRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
