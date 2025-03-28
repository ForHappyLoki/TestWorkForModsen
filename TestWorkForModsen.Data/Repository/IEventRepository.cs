using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Repository.BasicInterfaces;

namespace TestWorkForModsen.Data.Repository
{
    public interface IEventRepository<T> : IRepository<T>, ISingleKey<T> where T : class
    {
        //Интерфейс-объеденитель нескольких других
    }
}
