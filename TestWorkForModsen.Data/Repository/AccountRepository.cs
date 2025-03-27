using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class AccountRepository(DatabaseContext context) : IRepository<Account>
    {
        private readonly DatabaseContext _context = context;

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Account.ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Account.FindAsync(id);
        }
        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Account
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account != null)
            {
                _context.Account.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Account>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Account
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод DeleteByCompositeKeyAsync не поддерживается для Account.");
        }

        public async Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод GetByCompositeKeyAsync не поддерживается для Account.");
        }

}
}
