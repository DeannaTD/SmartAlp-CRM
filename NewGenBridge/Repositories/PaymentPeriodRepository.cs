using NewGenBridge.Data;
using NewGenBridge.Models;
namespace NewGenBridge.Repositories
{
    public class PaymentPeriodRepository : BaseRepository<PaymentPeriod>
    {
        public PaymentPeriodRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
