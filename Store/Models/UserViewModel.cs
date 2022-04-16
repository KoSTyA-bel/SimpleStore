using System.ComponentModel.DataAnnotations;

namespace Store.Models;

public class UserViewModel
{
    public UserViewModel()
    {
        Login = string.Empty;
        Password = string.Empty;
    }

    public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо указать логин")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Необходимо указать пароль")]
    public string Password { get; set; }
}
