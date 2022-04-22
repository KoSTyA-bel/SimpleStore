using Store.BLL.Entities;

namespace Store.Hubs;

public interface ISales
{
    public Task StartSales(string product);

    public Task ProductDataChanged(Product product);
}
