using Store;
using Store.DLL.Contexts;
using Store.DLL.Repositories;
using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
            });

// Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer("Data Source=DESKTOP-GJHC42N;Initial Catalog=SimpleStore;Integrated Security=True"));
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IService<User>, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
