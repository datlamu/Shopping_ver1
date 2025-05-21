using Microsoft.AspNetCore.Identity;

namespace Shopping_ver1.Models
{
    public class UserModel : IdentityUser
    {
        public string Occupation { get; set; }
        public UserModel() { }
        public UserModel(string UserName, string Email)
        {
            this.UserName = UserName;
            this.Email = Email;
        }
    }
}
