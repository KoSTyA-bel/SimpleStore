namespace Store.BLL.Entities;

public class User
{
    public User()
    {
        Login = string.Empty;
        Password = Array.Empty<byte>();
    }

    public int Id { get; set; }

    public string Login { get; set; }

    public byte[] Password { get; set; }
}
