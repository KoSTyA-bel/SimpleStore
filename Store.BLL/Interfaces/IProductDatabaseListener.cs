namespace Store.BLL.Interfaces;

public interface IProductDatabaseListener
{
    Task BuyProduct(string user, string productId);
}
