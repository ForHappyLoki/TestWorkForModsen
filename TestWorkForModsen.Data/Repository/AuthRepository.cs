using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;

namespace TestWorkForModsen.Data.Repository
{
    public class AuthRepository(DatabaseContext databaseContext) : IAuthRepository<RefreshToken>
    {
        private readonly DatabaseContext _databaseContext = databaseContext;

        public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _databaseContext.RefreshTokens.Add(refreshToken);
            await _databaseContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _databaseContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            _databaseContext.RefreshTokens.Update(refreshToken);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}