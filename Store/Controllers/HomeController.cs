using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using AutoMapper;
using Store.Models;

namespace Store.Controllers;

public class HomeController : Controller
{
    private readonly IService<User> _service;
    private readonly IMapper _mapper;

    public HomeController(IService<User> service, IMapper mapper)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var users = await _service.GetRange(0, int.MaxValue);
        var view = _mapper.Map<IEnumerable<UserViewModel>>(users);
        return View(view);
    }
}
