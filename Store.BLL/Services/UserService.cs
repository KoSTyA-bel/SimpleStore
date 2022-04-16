using Store.BLL.Entities;
using Store.BLL.Interfaces;

namespace Store.BLL.Services;

public class UserService : IService<User>
{
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<User> Create(User entity)
    {
        await _repository.Create(entity);
        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _repository.GetById(id);

        await _repository.Delete(entity);

        return entity is null;
    }

    public bool TryGetById(int id, out User? entity)
    {
        entity = _repository.GetById(id).GetAwaiter().GetResult();
        return entity is not null;
    }

    public Task<IEnumerable<User>> GetRange(int startId, int count) => _repository.GetRange(startId, count);

    public async Task<User> Update(User entity)
    {
        await _repository.Update(entity);
        return entity;
    }

    public bool TryGetByName(string name, out User? entity)
    {
        entity = _repository.GetByName(name).GetAwaiter().GetResult();
        entity.Role = _repository
        return entity is not null;
    }
}
