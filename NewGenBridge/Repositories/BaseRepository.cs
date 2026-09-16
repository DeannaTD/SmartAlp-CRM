
using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;

namespace NewGenBridge.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity); ;
        }

        public virtual async ValueTask<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetCollectionAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
