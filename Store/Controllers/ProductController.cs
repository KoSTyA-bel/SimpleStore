using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.Interfaces;
using Store.BLL.Entities;
using AutoMapper;
using Store.Models;
using MongoDB.Bson;

namespace Store.Controllers;


public class ProductController : Controller
{
    private readonly IService<Product> _service;
    private readonly IMapper _mapper;

    public ProductController(IService<Product> service, IMapper mapper)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IActionResult> Index()
    {
        var products = await _service.GetRange(0, int.MaxValue);
        return View(_mapper.Map<IEnumerable<ProductViewModel>>(products));
    }

    [Route("{controller}/{id}")]
    public async Task<IActionResult> Product(string id)
    {
        var entityId = string.IsNullOrEmpty(id) ? MongoDB.Bson.ObjectId.Empty : MongoDB.Bson.ObjectId.Parse(id);

        if (!_service.TryGetById(entityId, out Product? product))
        {
            return View(null);
        }

        return View(_mapper.Map<ProductViewModel>(product));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = _mapper.Map<Product>(model);
        
        if (product is null)
        {
            return View(model);
        }

        await _service.Create(product);

        return RedirectToAction("Index");
    }
}
