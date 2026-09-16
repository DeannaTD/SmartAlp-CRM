using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class HubFileRoleRepository : BaseRepository<HubFileRole>
    {
        public HubFileRoleRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<IEnumerable<HubFileRole>> GetRolesByFileIdAsync(int fileId)
        {
            return await _context.HubFileRoles
                                 .Where(r => r.FileId == fileId)
                                 .ToListAsync();
        }
    }
}
