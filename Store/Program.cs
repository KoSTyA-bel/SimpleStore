using Store;
using Store.DLL.Contexts;
using Store.DLL.Repositories;
using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Store.DLL.Settings;
using Microsoft.Extensions.Options;
using Store.Hubs;
using Store.DLL.Listeners;
using Store.DataTransferLevel.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(opt => 
{
    opt.EnableDetailedErrors = true;
    opt.ClientTimeoutInterval = TimeSpan.MaxValue;
    opt.HandshakeTimeout = TimeSpan.MaxValue;
});

builder.Services.AddHostedService<ProductDatabaseListener>();

builder.Services.Configure<ProductDatabaseSettings>(builder.Configuration.GetSection(nameof(ProductDatabaseSettings)));
builder.Services.Configure<RabbitSettings>(builder.Configuration.GetSection(nameof(RabbitSettings)));

builder.Services.AddSingleton<ProductDatabaseSettings>(sp => sp.GetRequiredService<IOptions<ProductDatabaseSettings>>().Value);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
            });

// Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UsersData")));
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
builder.Services.AddSingleton<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IService<User>, UserService>();
builder.Services.AddScoped<IService<Product>, ProductService>();
builder.Services.AddScoped<ProductDatabaseListener>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<SalesHub>("/salesHub");

app.Run();
