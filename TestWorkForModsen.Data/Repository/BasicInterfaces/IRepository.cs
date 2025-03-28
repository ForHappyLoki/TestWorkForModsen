using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Repository.BasicInterfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); 
        Task AddAsync(T entity);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
