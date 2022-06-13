using Store.BLL.Entities;

namespace Store.Hubs;

/// <summary>
/// Provides methods implemented on the cient side.
/// </summary>
public interface ISales
{
    /// <summary>
    /// Notifies the client about the start of sales.
    /// </summary>
    /// <param name="product">Product id.</param>
    /// <returns>Task.</returns>
    public Task StartSales(string productId);

    /// <summary>
    /// Notifies the client of product data changes.
    /// </summary>
    /// <param name="product">Specific product.</param>
    /// <returns>Task.</returns>
    public Task ProductDataChanged(Product product);
}
