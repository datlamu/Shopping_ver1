using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Hãy nhập Email")]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public LoginViewModel() { }
    }
}
