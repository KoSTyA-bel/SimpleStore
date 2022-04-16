namespace Store.BLL.Interfaces;

public interface IService<T> where T : class
{
    public Task<bool> TryGetById(int id, out T entity);

    public Task<T> Create(T entity);

    public Task<T> Update(T entity);

    public Task<bool> Delete(int id);

    public Task<bool> TryGetRange(int startId, int count, out IEnumerable<T> entities);
}
