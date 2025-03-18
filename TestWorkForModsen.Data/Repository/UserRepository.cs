using Microsoft.EntityFrameworkCore;
using TestWork_Events.Data;
using TestWork_Events.Models;

namespace TestWork_Events.Repository
{
    public class UserRepository(DatabaseContext context) : IRepository<User>
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

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
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
        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод DeleteByCompositeKeyAsync не поддерживается для User.");
        }
        public async Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод GetByCompositeKeyAsync не поддерживается для User.");
        }
    }
}
