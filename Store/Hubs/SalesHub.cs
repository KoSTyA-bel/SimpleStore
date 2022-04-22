using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Store.BLL.Entities;
using Store.DLL.Listeners;
using System.Security.Claims;

namespace Store.Hubs;

public class SalesHub : Hub<ISales>
{
    private readonly ProductDatabaseListener _listener;

    public SalesHub(ProductDatabaseListener listener)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
    }

    public async Task ListenProduct(string productId)
    {
        await this.Groups.AddToGroupAsync(Context.ConnectionId, productId);
    }

    [Authorize]
    public async Task BuyProduct(string productId)
    {
        var name = Context.User.Identity.Name;
        _listener.BuyProduct(name, productId);
    }
}
