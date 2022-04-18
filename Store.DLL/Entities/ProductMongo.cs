using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Store.DLL.Entities;

public class ProductMongo
{
    public ProductMongo()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    [BsonId]
    public ObjectId Id { get; set; }
    
    public string Name { get; set; }

    public string Description { get; set; }

    public int Count { get; set; }

    public DateTime Created { get; init; }

    public DateTime StartOfSales { get; set; }
}
