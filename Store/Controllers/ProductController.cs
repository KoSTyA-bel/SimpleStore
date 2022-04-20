using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.Models;

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
        if (!MongoDB.Bson.ObjectId.TryParse(id, out MongoDB.Bson.ObjectId entityId))
        {
            entityId = MongoDB.Bson.ObjectId.Empty;
        }

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

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out MongoDB.Bson.ObjectId entityId))
        {
            entityId = MongoDB.Bson.ObjectId.Empty;
        }

        if (!_service.TryGetById(entityId, out Product? product))
        {
            return View(null);
        }

        return View(_mapper.Map<ProductViewModel>(product));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _service.Update(_mapper.Map<Product>(model));

        return RedirectToAction("Index");
    }
}
