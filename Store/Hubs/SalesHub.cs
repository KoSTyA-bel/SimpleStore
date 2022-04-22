using Microsoft.AspNetCore.SignalR;
using Store.BLL.Entities;
using Store.DLL.Listeners;
using System.Security.Claims;

namespace Store.Hubs;

public class SalesHub : Hub<ISales>
{
    public async Task ListenProduct(string productId)
    {
        await this.Groups.AddToGroupAsync(Context.ConnectionId, productId);
    }

    private async Task ProductStartSales(Product product)
    {
        await this.Clients.Group(product.Id).StartSales(product);
        // await this.Clients.All.SendAsync("StartSales", product.Id);
    }

    private async Task ProductDataChanged(Product product)
    {
        await this.Clients.Group(product.Id).ProductDataChanged(product);
    }
}
