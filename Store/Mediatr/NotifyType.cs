namespace Store.Mediatr;

[Flags]
public enum NotifyType
{
    CreateUser,
    CreateProduct,
    CreateRole,
    ReadUser,
    ReadProduct,
    ReadRole,
    UpdateUser,
    UpdateProduct,
    UpdateRole,
    DeleteUser,
    DeleteProduct,
    DeleteRole,
}
