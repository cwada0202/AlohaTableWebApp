using System.ComponentModel.DataAnnotations;

namespace WebAppECartCore.Models;

public class Login
{
    [Key]
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}
