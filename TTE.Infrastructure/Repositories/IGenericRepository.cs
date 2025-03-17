using System.Linq.Expressions;

namespace TTE.Infrastructure.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> Get();
        Task<T?> GetById(int id);
        Task<T?> GetByCondition(Expression<Func<T, bool>> predicate);
        Task<int> Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
