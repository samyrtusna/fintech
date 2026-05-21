using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Infrastructure.EFcore.ContextDb;
using fintech.API.Infrastructure.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;


namespace fintech.API.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        } 

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                var mapped = DatabaseExceptionMapper.Map(ex);

                if (mapped != ex)
                    throw mapped;

                throw;
            }
        }
    } 
}
