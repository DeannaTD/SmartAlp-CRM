using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class TranslationRepository : BaseRepository<Translation>
    {
        public TranslationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Translation GetByKey(string npKey)
        {
            return _dbSet.FirstOrDefault(t => t.NPKey == npKey);
        }
    }
}
