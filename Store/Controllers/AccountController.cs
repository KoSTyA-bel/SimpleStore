using Microsoft.AspNetCore.Mvc;
using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.Models;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Store.BLL;

namespace Store.Controllers;

public class AccountController : Controller
{
    private readonly IService<User> _service;
    private readonly IMapper _mapper;

    public AccountController(IService<User> service, IMapper mapper)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _mapper.Map<User>(model);

        if (!_service.TryGetByName(user.Login, out User? u))
        {
            ModelState.AddModelError("", "Данного пользователя не существует.");
            return View(model);
        }

        if (!Enumerable.SequenceEqual(u.Password, model.Password.GetMD5Hash()))
        {
            ModelState.AddModelError("", "Проверьте введённый пароль.");
            return View(model);
        }

        await Authenticate(u);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _mapper.Map<User>(model);
        await _service.Create(user);

        return RedirectToAction("Login", "Account");
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    protected async Task Authenticate(User user)
    {
        //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

        //identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
        //identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));

        //var principal = new ClaimsPrincipal(identity);

        //var authProperties = new AuthenticationProperties
        //{
        //    AllowRefresh = true,
        //    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
        //    IsPersistent = true,
        //};

        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);

        var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
            };

        var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
}
