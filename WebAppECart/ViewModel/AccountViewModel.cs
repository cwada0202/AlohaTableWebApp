using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppECart.ViewModel
{
    public class AccountViewModel
    {
        public int UserId { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required.")]
        public string UserPassword { get; set; }

        public string LoginErrorMessage { get; set; }
    }
}