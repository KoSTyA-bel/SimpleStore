using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Store;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.BLL.Services;
using Store.DataTransferLevel;
using Store.DataTransferLevel.Settings;
using Store.DLL.Contexts;
using Store.DLL.Listeners;
using Store.DLL.Repositories;
using Store.DLL.Settings;
using Store.Hubs;

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
builder.Services.AddSingleton<RabbitSettings>(sp => sp.GetRequiredService<IOptions<RabbitSettings>>().Value);

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
builder.Services.AddSingleton<IDataSender, DataSender>(x =>
{
    using (x.CreateScope())
    {
        var settings = x.GetService(typeof(RabbitSettings)) as RabbitSettings;
        return new DataSender(settings);
    }
});

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

var listener = new DataListener(app.Services);

Task.Run(() => listener.StartListen());

app.Run();
