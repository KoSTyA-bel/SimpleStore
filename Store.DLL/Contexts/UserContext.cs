using Microsoft.EntityFrameworkCore;
using Store.BLL.Entities;

namespace Store.DLL.Contexts;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.Id);
        modelBuilder.Entity<Role>().HasKey(role => role.Id);

        modelBuilder.Entity<User>().HasOne(user => user.Role).WithMany(role => role.Users).HasForeignKey(user => user.RoleId);

        var admin = new Role() { Id = 1, Name = "admin" };
        var user = new Role() { Id = 2, Name = "user" };

        modelBuilder.Entity<Role>().HasData(new Role[] { admin, user });

        base.OnModelCreating(modelBuilder);
    }
}
