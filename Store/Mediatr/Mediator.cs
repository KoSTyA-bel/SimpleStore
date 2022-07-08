using Store.BLL.Interfaces;
using Store.BLL.Entities;

namespace Store.Mediatr;

public class Mediator : IMediator
{
    private readonly IService<Product> productService;
    private readonly IService<User> userService;

    public Mediator(IService<Product> productService, IService<User> userService)
    {
        this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public object Send(CommandType commandType, EntityType entityType, object obj)
    {
        throw new NotImplementedException();
    }
}
