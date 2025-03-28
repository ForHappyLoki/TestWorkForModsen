using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkForModsen.Data.Repository.BasicInterfaces
{
    public interface IGetByEmailAsync<T> where T : class
    {
        Task<T> GetByEmailAsync(string email);
    }
}
