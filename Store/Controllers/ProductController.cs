using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.Mediatr;
using Store.Models;

namespace Store.Controllers;

public class ProductController : Controller
{
    private readonly IValidator<ProductViewModel> validator;
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public ProductController(IValidator<ProductViewModel> validator, IMapper mapper, IMediator mediator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IActionResult> Index()
    {
        var products = this.mediator.Send(CommandType.Read, EntityType.Product, 0) as IEnumerable<Product>;
        return View(mapper.Map<IEnumerable<ProductViewModel>>(products));
    }

    [Route("{controller}/{id}")]
    public async Task<IActionResult> Product(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out MongoDB.Bson.ObjectId entityId))
        {
            entityId = MongoDB.Bson.ObjectId.Empty;
        }

        var product = this.mediator.Send(CommandType.Read, EntityType.Product, entityId) as Product;

        if (product is null)
        {
            return View(null);
        }

        return View(mapper.Map<ProductViewModel>(product));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    [Route("{controller}/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    [Route("{controller}/Create")]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = mapper.Map<Product>(model);

        if (product is null)
        {
            return View(model);
        }

        this.mediator.Send(CommandType.Create, EntityType.Product, product);

        //await _service.Create(product);

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

        var product = this.mediator.Send(CommandType.Read, EntityType.Product, entityId) as Product;

        if (product is null)
        {
            return View(null);
        }

        return View(mapper.Map<ProductViewModel>(product));
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

        if (!this.validator.Validate(model).IsValid)
        {

        }

        this.mediator.Send(CommandType.Update, EntityType.Product, mapper.Map<Product>(model));

        return RedirectToAction("Index");
    }
}
