namespace TestWorkForModsen.Data.Repository.BasicInterfaces
{
    public interface IGetAll<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<T>> GetAllByEventIdAsync(int eventId);
    }
}
