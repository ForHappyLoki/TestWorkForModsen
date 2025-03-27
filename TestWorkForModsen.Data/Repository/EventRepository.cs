using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class EventRepository(DatabaseContext context) : IRepository<Event>
    {
        private readonly DatabaseContext _context = context;
        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Event.ToListAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Event.FindAsync(id);
        }

        //Заглушка
        public async Task<Event> GetByEmailAsync(string email)
        {
            throw new System.NotImplementedException("Метод GetByEmailAsync не поддерживается для Event.");

        }

        public async Task AddAsync(Event _event)
        {
            await _context.Event.AddAsync(_event);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event _event)
        {
            _context.Entry(_event).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var _event = await _context.User.FirstOrDefaultAsync(e => e.Id == id);
            if (_event != null)
            {
                _context.User.Remove(_event);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Event
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод DeleteByCompositeKeyAsync не поддерживается для Event.");
        }

        public async Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId)
        {
            throw new System.NotImplementedException("Метод GetByCompositeKeyAsync не поддерживается для Event.");
        }
    }
}
