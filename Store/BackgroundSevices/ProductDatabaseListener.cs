using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.DLL.Entities;
using Store.DLL.Settings;
using Store.Hubs;

namespace Store.DLL.Listeners;

public class ProductDatabaseListener : BackgroundService, IProductDatabaseListener
{
    private readonly object _locker = new object();
    private readonly IHubContext<SalesHub, ISales> _hub;
    private readonly ProductDatabaseSettings _settings;
    private readonly IMapper _mapper;
    private IMongoCollection<ProductMongo> _collection = null!;

    public ProductDatabaseListener(ProductDatabaseSettings settings, IHubContext<SalesHub, ISales> hub, IMapper mapper)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _hub = hub ?? throw new ArgumentNullException(nameof(hub));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        InitialazeCollection();
    }

    public async Task BuyProduct(string user, string productId)
    {
        lock (_locker)
        {
            var product = _collection.Find(x => x.Id == MongoDB.Bson.ObjectId.Parse(productId)).First();

            if (product.Count > 0)
            {
                product.Count--;
            }

            _collection.ReplaceOneAsync(x => x.Id == product.Id, product);
            _hub.Clients.Group(productId).ProductDataChanged(_mapper.Map<Product>(product));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var products = (await _collection.FindAsync(product => !product.IsSalesStart)).ToEnumerable();
            var date = DateTime.Now;
            List<Task> tasks = new List<Task>();

            foreach (var product in products)
            {
                if (product.IsSalesStart)
                {
                    continue;
                }

                if (product.StartOfSales <= date)
                {
                    product.IsSalesStart = true;
                    await _hub.Clients.Group(product.Id.ToString()).StartSales(product.Id.ToString());
                    tasks.Add(_collection.ReplaceOneAsync(x => x.Id == product.Id, product));
                }
            }

            Task.WaitAll(tasks.ToArray());

            products = null;

            await Task.Delay(1000);
        }
    }

    protected virtual void InitialazeCollection()
    {
        try
        {
            var client = new MongoClient($"mongodb://{_settings.Host}:{_settings.Port}");
            var db = client.GetDatabase(_settings.DatabaseName);
            _collection = db.GetCollection<ProductMongo>(_settings.CollectionName);

        }
        catch (Exception e)
        {
            var client = new MongoClient($"mongodb://{_settings.Login}:{_settings.Passord}@{_settings.Host}:{_settings.Port}");
            var db = client.GetDatabase(_settings.DatabaseName);
            _collection = db.GetCollection<ProductMongo>(_settings.CollectionName);
        }
    }
}
