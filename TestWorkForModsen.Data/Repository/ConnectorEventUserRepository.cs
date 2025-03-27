using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class ConnectorEventUserRepository(DatabaseContext context) : IConnectorEventUserRepository<ConnectorEventUser>
    {
        private readonly DatabaseContext _context = context;

        // Получить все записи
        public async Task<IEnumerable<ConnectorEventUser>> GetAllAsync()
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .ToListAsync();
        }

        // Получить запись по составному ключу (EventId и UserId)
        public async Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .FirstOrDefaultAsync(ceu => ceu.EventId == eventId && ceu.UserId == userId);
        }

        // Получить все записи для конкретного пользователя по UserId
        public async Task<IEnumerable<ConnectorEventUser>> GetAllByUserIdAsync(int userId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .Where(ceu => ceu.UserId == userId)
                .ToListAsync();
        }

        // Получить все записи для конкретного события по EventId
        public async Task<IEnumerable<ConnectorEventUser>> GetAllByEventIdAsync(int eventId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .Where(ceu => ceu.EventId == eventId)
                .ToListAsync();
        }

        // Добавить новую запись
        public async Task AddAsync(ConnectorEventUser entity)
        {
            await _context.ConnectorEventUser.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Обновить запись
        public async Task UpdateAsync(ConnectorEventUser entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Удалить запись по составному ключу (EventId и UserId)
        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            var entity = await _context.ConnectorEventUser
                .FirstOrDefaultAsync(ceu => ceu.EventId == eventId && ceu.UserId == userId);

            if (entity != null)
            {
                _context.ConnectorEventUser.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Получить записи с пагинацией
        public async Task<IEnumerable<ConnectorEventUser>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .OrderBy(ceu => ceu.AdditionTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Остальные методы (GetByIdAsync, GetByEmailAsync, DeleteAsync) остаются без изменений
        public async Task<ConnectorEventUser> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException("Используйте метод GetByCompositeKeyAsync вместо GetByIdAsync.");
        }

        public async Task<ConnectorEventUser> GetByEmailAsync(string email)
        {
            throw new System.NotImplementedException("Метод GetByEmailAsync не поддерживается для ConnectorEventUser.");
        }

        public async Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException("Используйте метод DeleteByCompositeKeyAsync вместо DeleteAsync.");
        }
    }
}