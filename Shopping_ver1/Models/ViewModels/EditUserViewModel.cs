using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_ver1.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }

        public EditUserViewModel() { }

        public EditUserViewModel(UserModel user, string roleId, IEnumerable<SelectListItem> roles)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            RoleId = roleId;
            Roles = roles;
        }
    }
}
