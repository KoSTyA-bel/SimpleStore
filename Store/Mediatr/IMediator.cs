namespace Store.Mediatr;

public interface IMediator
{
    public object Send(CommandType commandType, EntityType entityType, object obj);
}
