using Microsoft.AspNetCore.Identity;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services
{
    public interface IUserService
    {
        // Đăng nhập
        Task<SignInResult> LoginAsync(LoginViewModel userModel);

        // Đăng ký
        Task<IdentityResult> RegisterAsync(RegisterViewModel userModel, string role = "User");

        // Lấy thông tin 
        Task<EditUserViewModel> GetEditUserViewModelAsync(string userId);

        // Chỉnh sửa thông tin
        Task<(bool Success, string Message)> EditUserAsync(EditUserViewModel model);

        // Xóa user
        Task<(bool Success, string Message)> DeleteUserAsync(string userId);

        // Đăng xuất
        Task LogoutAsync();
    }
}
