using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TTE.Infrastructure.Data;


namespace TTE.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _entity;

        public async Task<T?> GetByCondition(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _entity.AsQueryable();

            // Apply eager loading for related entities
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public async Task<int> Add(T entity)
        {
            await _entity.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _entity.FindAsync(id);
            if (entity != null)
            {
                _entity.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> Get()
        {
            return await _entity.AsNoTracking().ToListAsync();
        }

        public async Task Update(T entity)
        {
            _entity.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
