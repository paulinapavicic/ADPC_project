using Microsoft.EntityFrameworkCore;
using Project_1.Data;

namespace Project_1.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MedicalDbContext _context;

        public Repository(MedicalDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
    }
}

