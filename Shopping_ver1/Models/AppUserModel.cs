using Microsoft.AspNetCore.Identity;

namespace Shopping_ver1.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
    }
}
