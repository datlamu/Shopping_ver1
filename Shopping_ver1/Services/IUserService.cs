using Microsoft.AspNetCore.Identity;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services
{
    public interface IUserService
    {
        // Đăng nhập tài khoản
        Task<SignInResult> LoginAsync(LoginViewModel userModel);

        // Đăng ký tài khoản
        Task<IdentityResult> RegisterAsync(RegisterViewModel userModel);

        // Đăng xuất
        Task LogoutAsync();
    }
}
