namespace Store.BLL.Interfaces;

public interface IService<T> where T : class
{
    public bool TryGetById(object entityId, out T? entity);

    public bool TryGetByName(string name, out T? entity);

    public Task<T?> Create(T entity);

    public Task<T> Update(T entity);

    public Task<bool> Delete(int id);

    public Task<IEnumerable<T>> GetRange(int startId, int count);
}
