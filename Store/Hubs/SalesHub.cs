using Microsoft.AspNetCore.SignalR;
using Store.BLL.Entities;
using Store.DLL.Listeners;

namespace Store.Hubs;

public class SalesHub : Hub
{
    private readonly ProductDatabaseListener _listener;

    public SalesHub(ProductDatabaseListener listener)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        _listener.OnProductStartSales += (s, p) => ProductStartSales(p);
        _listener.OnProductDataChanged += (s, p) => ProductDataChanged(p);
    }

    public async Task ListenProduct(string productId)
    {
        await this.Groups.AddToGroupAsync(Context.ConnectionId, productId);
    }

    public async Task BuyProduct(string productId)
    {

    }

    private async Task ProductStartSales(Product product)
    {
        await this.Clients.Group(product.Id).SendAsync("StartSales");
    }

    private async Task ProductDataChanged(Product product)
    {
        await this.Clients.Group(product.Id).SendAsync("DataChanged", product);
    }
}
