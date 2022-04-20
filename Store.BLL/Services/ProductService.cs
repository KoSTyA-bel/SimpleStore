using Store.BLL.Interfaces;
using Store.BLL.Entities;

namespace Store.BLL.Services;

public class ProductService : IService<Product>
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task<Product?> Create(Product entity)
    {
        _repository.Create(entity).GetAwaiter().GetResult();
        return _repository.GetByName(entity.Name);
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetRange(int startId, int count) => _repository.GetRange(startId, count);

    public bool TryGetById(object id, out Product? entity)
    {
        entity = _repository.GetById(id).GetAwaiter().GetResult();

        return entity is not null;
    }

    public bool TryGetByName(string name, out Product? entity)
    {
        entity = _repository.GetByName(name).GetAwaiter().GetResult();
        return entity is not null;
    }

    public Task<Product> Update(Product entity)
    {
        throw new NotImplementedException();
    }
}
