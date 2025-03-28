using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Repository.BasicInterfaces
{
    public interface ISingleKey<T> where T : class
    {
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(int id);
    }
}
