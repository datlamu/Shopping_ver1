using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Hãy nhập tên người dùng")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email"), EmailAddress]
        public string Email { get; set; }
        [RegularExpression(@"^\d*$", ErrorMessage = "Số điện thoại chỉ được chứa số")]
        public string PhoneNumber { get; set; }
    }
}
