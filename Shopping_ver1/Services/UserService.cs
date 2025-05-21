using Microsoft.AspNetCore.Identity;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services
{
    public class UserService : IUserService
    {
        // Identity managers ( Quản lý đăng nhập và người dùng )
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        // Inject Identity managers
        public UserService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Đăng nhập tài khoản
        public async Task<SignInResult> LoginAsync(LoginViewModel user)
        {
            return await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, false);
        }

        // Đăng ký tài khoản
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel user)
        {
            var userModel = new UserModel
            {
                UserName = user.Username,
                Email = user.Email
            };

            return await _userManager.CreateAsync(userModel, user.Password);
        }

        // Đăng xuất
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
