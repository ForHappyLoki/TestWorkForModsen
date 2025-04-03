using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class AccountRepository(DatabaseContext context) : IAccountRepository<Account>
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

        public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
        {
            _context.Account.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Account>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Account
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
