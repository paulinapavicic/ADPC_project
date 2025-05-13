namespace Project_1.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
