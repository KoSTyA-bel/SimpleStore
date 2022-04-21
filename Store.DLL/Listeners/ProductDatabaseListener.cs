using Microsoft.Extensions.DependencyInjection;
using Store.BLL.Entities;
using Store.BLL.Interfaces;

namespace Store.DLL.Listeners;

public class ProductDatabaseListener
{
    private readonly IRepository<Product> _repository;
    private readonly object _locker = new object();

    public ProductDatabaseListener(IServiceProvider serviceProvider)
    {
        using(var scope = serviceProvider.CreateScope())
        {
            _repository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>() ?? throw new ArgumentNullException(nameof(_repository));
        }
    }

    public event EventHandler<Product> OnProductStartSales;
    public event EventHandler<Product> OnProductDataChanged;

    public async Task BuyProduct(string productId)
    {
        lock (_locker)
        {
            var product = _repository.GetById(productId).GetAwaiter().GetResult();

            if (product.Count > 0)
            {
                product.Count--;
            }

            OnProductDataChanged?.Invoke(this, product);

            _repository.Update(product).GetAwaiter().GetResult();
        }
    }

    // Need better performance...
    public async Task ListenMongo()
    {
        while (true)
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
                    OnProductStartSales?.Invoke(this, product);
                    tasks.Add(_repository.Update(product));
                }
            }

            Task.WaitAll(tasks.ToArray());

            await Task.Delay(1000);
        }
    }
}
