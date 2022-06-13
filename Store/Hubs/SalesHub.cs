using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Store.BLL.Interfaces;
using Store.DLL.Listeners;

namespace Store.Hubs;

/// <summary>
/// provides methods to be called by clients.
/// </summary>
public class SalesHub : Hub<ISales>
{
    private readonly IProductDatabaseListener _listener;

    /// <summary>
    /// Initialize a new instanse of the <see cref="SalesHub"/>.
    /// </summary>
    /// <param name="listener">Product database listener.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> in null.</exception>
    public SalesHub(ProductDatabaseListener listener)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
    }

    /// <summary>
    /// Adds client to the listening group.
    /// </summary>
    /// <param name="productId">Product id.</param>
    /// <returns>Task.</returns>
    public async Task ListenProduct(string productId)
    {
        await this.Groups.AddToGroupAsync(Context.ConnectionId, productId);
    }

    /// <summary>
    /// Makes a product purchase by the client.
    /// </summary>
    /// <param name="productId">Product id.</param>
    /// <returns>Task.</returns>
    [Authorize]
    public async Task BuyProduct(string productId)
    {
        var name = Context.User.Identity.Name;
        _listener.BuyProduct(name, productId);
    }
}
