using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.DLL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Store.DLL.Repositories;

public class RoleRepository : IRepository<Role>
{
    private readonly UserContext _context;

    public RoleRepository(UserContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task Create(Role entity)
    {
        _context.Roles.Add(entity);
        return _context.SaveChangesAsync();
    }

    public Task Delete(Role entity)
    {
        _context.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public Task<Role?> GetById(int id) => _context.Roles.Where(role => role.Id == id).FirstOrDefaultAsync();

    public Task<Role?> GetByName(string name) => _context.Roles.Where(role => role.Name.Equals(name)).FirstOrDefaultAsync();

    public Task<IEnumerable<Role>> GetRange(int startId, int count) => Task<IEnumerable<Role>>.Factory.StartNew(() =>
    {
        if (count <= 0)
        {
            return new List<Role>();
        }

        return _context.Roles.Where(role => role.Id >= startId && role.Id < startId + count);
    });

    public Task Update(Role entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _context.Attach(entity);
        }

        _context.Entry(entity).State = EntityState.Modified;

        return _context.SaveChangesAsync();
    }
}
