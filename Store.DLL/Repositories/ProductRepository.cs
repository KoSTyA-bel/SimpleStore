﻿using Store.BLL.Interfaces;
using Store.BLL.Entities;
using Store.DLL.Settings;
using AutoMapper;
using MongoDB.Driver;
using Store.DLL.Entities;

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
        return _collection.InsertOneAsync(product);
    }

    public Task Delete(Product entity) => _collection.DeleteOneAsync(x => x.Id.ToString().Equals(entity.Id));

    public Task<Product?> GetById(int id)
    {
        throw new NotImplementedException();
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

    public Task<IEnumerable<Product>> GetRange(int startId, int count)
    {
        throw new NotImplementedException();
    }

    public Task Update(Product entity)
    {
        throw new NotImplementedException();
    }

    protected virtual void InitialazeCollection()
    {
        var client = new MongoClient($"mongodb://{_settings.Login}:{_settings.Passord}@{_settings.Host}:{_settings.Port}");
        var db = client.GetDatabase(_settings.DatabaseName);
        _collection = db.GetCollection<ProductMongo>(_settings.CollectionName);
    }
}