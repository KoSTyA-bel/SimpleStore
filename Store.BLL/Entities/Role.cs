namespace Store.BLL.Entities;

public class Role
{
    public Role()
    {
        Name = string.Empty;
        Users = new List<User>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<User> Users { get; set; }
}
