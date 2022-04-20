using Microsoft.AspNetCore.SignalR;

namespace Store.Hubs;

public class SalesHub : Hub
{
    public async Task ListenProduct(string productId)
    {
        Console.WriteLine(productId);
        await this.Groups.AddToGroupAsync(Context.ConnectionId, productId);
    }
}
