using TestWorkForModsen.Data.Models;

namespace TestWorkForModsen.Data.Repository
{
    public interface IAuthRepository<T> where T : class
    {
        Task<T> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<T?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    }
}