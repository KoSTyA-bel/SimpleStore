namespace Store.BLL.Entities;

public class Product
{
    public Product()
    {
        Id = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Created = DateTime.UtcNow;
        StartOfSales = DateTime.UtcNow;
        IsSalesStart = false;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Count { get; set; }

    public DateTime Created { get; init; }

    public DateTime StartOfSales { get; set; }

    public bool IsSalesStart { get; set; }
}
