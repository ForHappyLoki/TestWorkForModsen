using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Repository.BasicInterfaces
{
    public interface ICompositeKey<T> where T : class
    {
        Task DeleteByCompositeKeyAsync(int eventId, int userId, CancellationToken cancellationToken = default);
        Task<T> GetByCompositeKeyAsync(int eventId, int userId);
    }
}
