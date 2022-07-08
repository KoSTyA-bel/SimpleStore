namespace Store.BLL.Interfaces
{
    public interface IComponent<T> where T : class
    {
        public Task Notify(CommandType command, T obj);
    }
}
