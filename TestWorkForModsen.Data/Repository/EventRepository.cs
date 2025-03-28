using Microsoft.EntityFrameworkCore;
using System.Threading;
using TestWorkForModsen.Data;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class EventRepository(DatabaseContext context) : IEventRepository<Event>
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

        public async Task AddAsync(Event _event)
        {
            await _context.Event.AddAsync(_event);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event _event, CancellationToken cancellationToken = default)
        {
            _context.Entry(_event).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var _event = await _context.User.FirstOrDefaultAsync(e => e.Id == id);
            if (_event != null)
            {
                _context.User.Remove(_event);
                await _context.SaveChangesAsync(cancellationToken);
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
    }
}
