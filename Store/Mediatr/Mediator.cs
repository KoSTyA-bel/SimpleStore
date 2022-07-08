﻿using Store.BLL.Interfaces;
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
            EntityType.User => DoActionWithUserService(commandType, obj),
            EntityType.Product => DoActionWithProductService(commandType, obj),
        };

    private object DoActionWithUserService(CommandType commandType, object obj)
    {
        switch (commandType)
        {
            case CommandType.Create:
                break;
            case CommandType.Read:
                break;
            case CommandType.Update:
                break;
            case CommandType.Delete:
                break;
        }

        throw new ArgumentException("The command cannot be executed");
    }

    private object DoActionWithProductService(CommandType commandType, object obj)
    {
        switch (commandType)
        {
            case CommandType.Create:
                break;
            case CommandType.Read:
                break;
            case CommandType.Update:
                break;
            case CommandType.Delete:
                break;
        }

        throw new ArgumentException("The command cannot be executed");
    }
}
