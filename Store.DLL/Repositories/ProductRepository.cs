using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.DLL.Settings;
using AutoMapper;
using MongoDB.Driver;
using Store.DLL.Entities;
using MongoDB.Bson;

namespace Store.DLL.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly ProductDatabaseSettings _settings;
    private readonly IMapper _mapper;
    private IMongoCollection<ProductMongo> _collection = null!;

    public ProductRepository(ProductDatabaseSettings settings, IMapper mapper)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        InitialazeCollection();
    }

    public Task Create(Product entity)
    {
        var product = _mapper.Map<ProductMongo>(entity);
        product.Id = default;
        return _collection.InsertOneAsync(product);
    }

    public Task Delete(Product entity) => _collection.DeleteOneAsync(x => x.Id.ToString().Equals(entity.Id));

    public async Task<Product?> GetById(object entityId)
    {
        try
        {
            var id = (ObjectId)entityId;
            var entities = await _collection.FindAsync(product => product.Id == id);
            var entity = await entities.FirstAsync();
            return _mapper.Map<Product>(entity);
        }
        catch (InvalidCastException e)
        {
            return null;
        }
    }

    public async Task<Product?> GetByName(string name)
    {
        var cursor = await _collection.FindAsync(x => x.Name.Equals(name));
        var entity = await cursor.FirstOrDefaultAsync();

        if (entity == null)
        {
            return _mapper.Map<Product>(entity);
        }

        return null;
    }

    public async Task<IEnumerable<Product>> GetRange(int startId, int count)
    {
        var entitites = await _collection.FindAsync(p => true);
        var products = entitites.ToEnumerable().Skip(startId).Take(count);
        return _mapper.Map<IEnumerable<Product>>(products);
    }

    public Task Update(Product entity)
    {
        throw new NotImplementedException();
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
