using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class HubFileRepository : BaseRepository<HubFile>
    {
        public HubFileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HubFile>> GetAllFilesByRoleAsync(string roleName)
        {
            return await
                _context.HubFiles
                    .Where(f => f.HubFileRoles.Any(r => r.RoleName == roleName))
                    .ToListAsync();
        }
    }
}
