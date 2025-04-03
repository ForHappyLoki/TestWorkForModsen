using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;

namespace TestWorkForModsen.Data.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DatabaseContext _context;

        public RefreshTokenRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.Account)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<RefreshToken> GetValidTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.Account)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.ExpiryTime > DateTime.UtcNow);
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
