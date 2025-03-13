namespace TTE.Infrastructure.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> Get();
        Task<int> Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
