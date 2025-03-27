using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class AuthRepository(DatabaseContext databaseContext) : IAuthRepository
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

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _databaseContext.RefreshTokens.Update(refreshToken);
            await _databaseContext.SaveChangesAsync();
        }
    }
}