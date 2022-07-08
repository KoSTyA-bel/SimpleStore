namespace Store.Mediatr;

[Flags]
public enum CommandType
{
    Create,
    Read,
    Update,
    Delete,
}
