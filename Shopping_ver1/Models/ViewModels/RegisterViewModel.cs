using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập Password")]
        public string Password { get; set; }
        public RegisterViewModel() { }
        public RegisterViewModel(string UserName, string Email, string Password)
        {
            Username = UserName;
            this.Email = Email;
            this.Password = Password;
        }
    }
}
