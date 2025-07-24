using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_ver1.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Hãy nhập Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email"), EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập Password")]
        public string Password { get; set; }
        public string RoleId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }

        public RegisterViewModel() { }
    }
}
