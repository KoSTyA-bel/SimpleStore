using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.Hubs;

namespace Store.DLL.Listeners;

public class ProductDatabaseListener : BackgroundService
{
    private readonly IRepository<Product> _repository;
    private readonly object _locker = new object();
    private readonly IHubContext<SalesHub, ISales> _hub;

    public ProductDatabaseListener(IRepository<Product> repository, IHubContext<SalesHub, ISales> hub)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _hub = hub ?? throw new ArgumentNullException(nameof(hub));
    }

    public async Task BuyProduct(string user, string productId)
    {
        lock (_locker)
        {
            var product = _repository.GetById(productId).GetAwaiter().GetResult();

            if (product.Count > 0)
            {
                product.Count--;
            }

            _repository.Update(product).GetAwaiter().GetResult();
            _hub.Clients.Group(productId).ProductDataChanged(product);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var products = await _repository.GetRange(0, int.MaxValue);
            var date = DateTime.Now;
            List<Task> tasks = new List<Task>();

            foreach (var product in products)
            {
                if (product.IsSalesStart)
                {
                    continue;
                }

                if (product.StartOfSales <= date)
                {
                    product.IsSalesStart = true;
                    await _hub.Clients.Group(product.Id).StartSales(product.Id);
                    tasks.Add(_repository.Update(product));
                }
            }

            Task.WaitAll(tasks.ToArray());

            await Task.Delay(1000);
        }
    }
}
