namespace NewGenBridge.Repositories
{
    public interface IRepository<T> where T : class
    {
        //CRUD
        Task CreateAsync(T entity);
        ValueTask<T> GetByIdAsync(object id);
        Task<List<T>> GetCollectionAsync();
        void Update(T entity);
        void Delete(T entity);
    }
}
