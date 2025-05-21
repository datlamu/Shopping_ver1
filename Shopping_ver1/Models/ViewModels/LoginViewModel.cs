using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập Username")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public LoginViewModel() { }
        public LoginViewModel(string UserName, string Password)
        {
            this.Username = UserName;
            this.Password = Password;
        }
    }
}
