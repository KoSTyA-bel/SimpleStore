namespace Store.BLL.Interfaces;

public interface IRepository<T> where T : class
{
    public Task<T?> GetById(int id, out T entity);

    public Task Create(T entity);

    public Task Update(T entity);

    public Task Delete(int id);

    public Task<List<T>> GetRange(int startId, int count);
}
