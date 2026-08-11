using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppECartCore.Models;

public class AccountViewModel
{
    [DisplayName("User Name")]
    [Required(ErrorMessage = "This field is required.")]
    public string UserName { get; set; } = string.Empty;

    [DisplayName("Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "This field is required.")]
    public string UserPassword { get; set; } = string.Empty;

    public string? LoginErrorMessage { get; set; }
}
