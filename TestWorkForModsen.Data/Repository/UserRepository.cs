using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;

namespace TestWorkForModsen.Repository
{
    public class UserRepository(DatabaseContext context) : IUserRepository<User>
    {
        private readonly DatabaseContext _context = context;
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.User
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
