using Microsoft.EntityFrameworkCore;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.DLL.Contexts;

namespace Store.DLL.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task Create(User entity) => Task.Run(() => _context.Users.Add(entity));

    public Task Delete(User entity) => Task.Run(() => _context.Remove<User>(entity));

    public Task<User?> GetById(int id) => _context.Users.Where(user => user.Id == id).FirstOrDefaultAsync();

    public Task<IEnumerable<User>> GetRange(int startId, int count) => Task<IEnumerable<User>>.Factory.StartNew(() =>
    {
        if (count <= 0)
        {
            return new List<User>();
        }

        return _context.Users.TakeWhile(user => user.Id >= startId && user.Id < startId + count);
    });

    public Task Update(User entity) => Task.Run(() =>
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _context.Attach(entity);
        }

        _context.Entry(entity).State = EntityState.Modified;
    });
}
