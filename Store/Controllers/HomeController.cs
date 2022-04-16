using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.Entities;
using Store.BLL.Interfaces;

namespace Store.Controllers;

public class HomeController : Controller
{
    private readonly IService<User> _service;

    public HomeController(IService<User> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View(await _service.GetRange(0, int.MaxValue));
    }
}
