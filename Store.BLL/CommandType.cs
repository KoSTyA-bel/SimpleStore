namespace Store.BLL;

[Flags]
public enum CommandType
{
    Create,
    Read,
    Update,
    Delete,
}
