﻿namespace Store.BLL.Interfaces;

public interface IRepository<T> where T : class
{
    public Task<T?> GetById(int id);

    public Task Create(T entity);

    public Task Update(T entity);

    public Task Delete(T entity);

    public Task<IEnumerable<T>> GetRange(int startId, int count);
}
