namespace TestWork_Events.Repository
{
    public interface IConnectorEventUserRepository<T> : IRepository<T>, IGetAll<T> where T : class
    {
        //Интерфейс-объеденитель двух других
    }
}