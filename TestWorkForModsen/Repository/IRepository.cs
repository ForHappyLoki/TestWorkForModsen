using TestWork_Events.Models;

namespace TestWork_Events.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); 
        Task<T> GetByIdAsync(int id);
        Task<T> GetByEmailAsync(string email);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task DeleteByCompositeKeyAsync(int eventId, int userId);
        Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId);
    }
}
