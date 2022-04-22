using Store.BLL.Entities;

namespace Store.Hubs;

public interface ISales
{
    public Task StartSales(Product product);

    public Task ProductDataChanged(Product product);

    public Task BuyProduct(string productId);
}
