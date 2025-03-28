using TestWorkForModsen.Data.Repository.BasicInterfaces;

namespace TestWorkForModsen.Repository
{
    public interface IConnectorEventUserRepository<T> : IRepository<T>, IGetAll<T>, ICompositeKey<T> where T : class
    {
        //Интерфейс-объеденитель нескольких других
    }
}