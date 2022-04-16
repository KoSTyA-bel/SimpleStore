namespace Store.BLL.Entities;

public class Product
{
    public Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        Created = DateTime.UtcNow;
        StartOfSales = DateTime.UtcNow;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Count { get; set; }

    public DateTime Created { get; init; }

    public DateTime StartOfSales { get; set; }
}
