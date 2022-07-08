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

    public object Send(CommandType commandType, EntityType entityType, object obj) =>
        entityType switch
        {
            EntityType.User => DoActionWithService(commandType, obj, this.userService),
            EntityType.Product => DoActionWithService(commandType, obj, this.productService),
        };

    private object DoActionWithService<T>(CommandType commandType, object obj, IService<T> service) where T: class
    {
        switch (commandType)
        {
            case CommandType.Create:
                return service.Create(obj as T).GetAwaiter().GetResult();
            case CommandType.Read:
                service.TryGetById(obj, out T entity);
                return entity;
            case CommandType.Update:
                return service.Update(obj as T).GetAwaiter().GetResult();
            case CommandType.Delete:
                // TODO:
                // Implement deleting.
                //return service.Delete(obj);
                break;
        }

        throw new ArgumentException("The command cannot be executed");
    }
}
