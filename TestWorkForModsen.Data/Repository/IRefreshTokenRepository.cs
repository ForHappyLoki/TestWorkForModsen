using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<RefreshToken> GetValidTokenAsync(string token);
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task DeleteExpiredTokensAsync();
    }
}
