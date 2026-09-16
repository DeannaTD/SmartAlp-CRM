using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
