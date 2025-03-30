using System.Linq.Expressions;

namespace TTE.Infrastructure.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T?> GetByCondition(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> GetEntityWithIncludes(params string[] includes);
        Task<IEnumerable<T>> GetAllByCondition(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> Get();
        Task<int> Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
