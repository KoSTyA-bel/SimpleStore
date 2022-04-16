using Store.BLL.Entities;
using Store.BLL.Interfaces;

namespace Store.BLL.Services;

public class UserService : IService<User>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Role> _roleRepository;

    public UserService(IRepository<User> userRepository, IRepository<Role> roleRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    }

    public async Task<User> Create(User entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var role = await _roleRepository.GetByName("user");

        if (role is null)
        {
            throw new ArgumentNullException(nameof(role), "Can`t find user role in DB.");
        }

        entity.Role = role;
        entity.RoleId = role.Id;
        await _userRepository.Create(entity);

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _userRepository.GetById(id);

        await _userRepository.Delete(entity);

        return entity is null;
    }

    public bool TryGetById(int id, out User? entity)
    {
        entity = _userRepository.GetById(id).GetAwaiter().GetResult();
        bool result;

        if (result = entity is not null)
        {
            var role = _roleRepository.GetById(entity.Id).GetAwaiter().GetResult();
            entity.Role = role ?? throw new ArgumentNullException(nameof(role), "Can`t find user role in DB.");
        }

        return result;
    }

    public Task<IEnumerable<User>> GetRange(int startId, int count) => _userRepository.GetRange(startId, count);

    public async Task<User> Update(User entity)
    {
        await _userRepository.Update(entity);
        return entity;
    }

    public bool TryGetByName(string name, out User? entity)
    {
        entity = _userRepository.GetByName(name).GetAwaiter().GetResult();

        bool result;

        if (result = entity is not null)
        {
            var role = _roleRepository.GetById(entity.Id).GetAwaiter().GetResult();
            entity.Role = role ?? throw new ArgumentNullException(nameof(role), "Can`t find user role in DB.");
        }
        
        return result;
    }
}
