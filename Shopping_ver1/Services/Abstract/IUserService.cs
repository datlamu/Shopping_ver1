using Microsoft.AspNetCore.Identity;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
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
        Task<OperationResult> EditUserAsync(EditUserViewModel model);

        // Xóa user
        Task<OperationResult> DeleteUserAsync(string userId);

        // Đăng xuất
        Task LogoutAsync();

        // Quên mật khẩu
        Task<string> ForgotPassword(string userEmail, string baseUrl);

        // Mật khẩu mới
        Task<OperationResult> NewPassword(NewPasswordViewModel newPassVM);

        // Tìm kiếm user từ email
        Task<UserViewModel> FindAccountByMailAsync(string userEmail);

        // Cập nhật thông tin tài khoản
        Task<OperationResult> UpdateAccountInfoAsync(UserViewModel userVM);
    }
}
