using Project_1.Data;

namespace Project_1.Repository
{
    public class RepositoryFactory
    {
        private readonly MedicalDbContext _context;

        public RepositoryFactory(MedicalDbContext context)
        {
            _context = context;
        }

        public IRepository<T> CreateRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }
    }
}
