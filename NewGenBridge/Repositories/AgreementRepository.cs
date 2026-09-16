using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class AgreementRepository : BaseRepository<Agreement>
    {
        public AgreementRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
