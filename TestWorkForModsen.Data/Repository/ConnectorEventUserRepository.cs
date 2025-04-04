﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestWorkForModsen.Data;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Repository
{
    public class ConnectorEventUserRepository(DatabaseContext context) 
        : IConnectorEventUserRepository<ConnectorEventUser>
    {
        private readonly DatabaseContext _context = context;

        public async Task<IEnumerable<ConnectorEventUser>> GetAllAsync()
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .ToListAsync();
        }

        public async Task<ConnectorEventUser> GetByCompositeKeyAsync(int eventId, int userId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .FirstOrDefaultAsync(ceu => ceu.EventId == eventId && ceu.UserId == userId);
        }

        public async Task<IEnumerable<ConnectorEventUser>> GetAllByUserIdAsync(int userId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .Where(ceu => ceu.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConnectorEventUser>> GetAllByEventIdAsync(int eventId)
        {
            return await _context.ConnectorEventUser
                .Include(ceu => ceu.Event)
                .Include(ceu => ceu.User)
                .Where(ceu => ceu.EventId == eventId)
                .ToListAsync();
        }

        public async Task AddAsync(ConnectorEventUser entity)
        {
            await _context.ConnectorEventUser.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ConnectorEventUser entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByCompositeKeyAsync(ConnectorEventUser entity, CancellationToken cancellationToken = default)
        {
            _context.ConnectorEventUser.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

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
    }
}