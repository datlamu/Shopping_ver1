using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Services.Implement
{
    public class UserService : IUserService
    {
        // Identity managers ( Quản lý đăng nhập và người dùng )
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        // Inject Identity managers
        public UserService(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        // Đăng nhập
        public async Task<SignInResult> LoginAsync(LoginViewModel user)
        {
            // Tìm người dùng theo email
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                // Nếu không tìm thấy email => thất bại
                return SignInResult.Failed;
            }

            var result = await _signInManager.PasswordSignInAsync(existingUser.UserName, user.Password, false, false);

            // Đăng nhập thành công ( test email )
            if (result.Succeeded)
            {
                var toEmail = user.Email;
                var subject = "Đăng nhập thành công !";
                var body = "Chúc bạn có trải nghiệm vui vẻ nhé !";
                _ = Task.Run(async () =>
                {
                    await _emailService.SendEmailAsync(toEmail, subject, body);
                });
            }

            return result;
        }

        // Đăng ký
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel user, string role = "User")
        {
            // Tạo user model
            var userModel = new UserModel
            {
                UserName = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            // Tạo tài khoản
            var result = await _userManager.CreateAsync(userModel, user.Password);

            // Kiểm tra kết quả 
            if (result.Succeeded)
            {
                // Gán role cho user
                await _userManager.AddToRoleAsync(userModel, role);
            }
            return result;
        }

        // Lấy thông tin
        public async Task<EditUserViewModel> GetEditUserViewModelAsync(string userId)
        {
            // Kiểm tra id
            if (string.IsNullOrEmpty(userId)) return null;

            // Kiểm tra user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            // Lấy danh sách role
            var roles = await _roleManager.Roles.ToListAsync();
            var selectListRoles = new SelectList(roles, "Id", "Name");

            // Tìm RoleId từ RoleName
            var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var roleId = selectListRoles.FirstOrDefault(r => r.Text == roleName)?.Value;

            // Trả về ViewModel
            return new EditUserViewModel(user, roleId, selectListRoles);
        }

        // Chỉnh sửa thông tin
        public async Task<OperationResult> EditUserAsync(EditUserViewModel model)
        {
            // Tìm kiểm user
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return new OperationResult(false, "Không tìm thấy user");

            // Lấy thông tin thay đổi
            user.Email = model.Email;
            user.UserName = model.Username;
            user.PhoneNumber = model.PhoneNumber;

            // Cập nhật
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new OperationResult(false, "Cập nhật user thất bại");

            // Role hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            // Tìm Role từ RoleId
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return new OperationResult(false, "Role không hợp lệ");

            // Nếu cập nhật role
            if (currentRole != role.Name)
            {
                // Xóa role cũ
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return new OperationResult(false, "Xóa role cũ thất bại");

                // Gán role mới
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                    return new OperationResult(false, "Gán role thất bại");
            }

            // Kết quả nếu thành công
            return new OperationResult(true, "Cập nhật thành công");
        }

        // Xóa user
        public async Task<OperationResult> DeleteUserAsync(string id)
        {
            // Kiểm tra Id
            if (string.IsNullOrEmpty(id))
                return new OperationResult(false, "ID không hợp lệ!");

            // Kiểm tra User
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new OperationResult(false, "Không tìm thấy thông tin user này!");

            // Lấy xóa Role của User
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!removeResult.Succeeded)
                    return new OperationResult(false, "Xóa role của người dùng thất bại!");
            }

            // Xóa User
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return new OperationResult(false, "Xóa người dùng thất bại!");

            // Kết quả nếu thành công
            return new OperationResult(true, "Xóa thành công!");
        }

        // Đăng xuất
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // Quên mật khẩu
        public async Task<string> ForgotPassword(string userEmail, string baseUrl)
        {
            // Tìm User
            var user = await _userManager.FindByEmailAsync(userEmail);

            // Nếu user tồn tại và đã xác thực email
            if (user != null)
            {
                // Tạo token và mã hóa token ( tránh lỗi đối với ký tự đặc biệt )
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string encodedToken = Uri.EscapeDataString(token);
                string encodedEmail = Uri.EscapeDataString(userEmail);

                // Tạo link
                string resetLink = $"{baseUrl}/Account/NewPassword?email={encodedEmail}&token={encodedToken}";

                // Nội dung mail
                string subject = $"Khôi phục mật khẩu tài khoản {user.Email}";
                string body = $@"
                    <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                    <p>Nhấn vào liên kết sau để đặt lại mật khẩu:</p>
                    <p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>
                    <p>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này.</p>";

                // Gửi mail đến chính email người dùng
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }

            return "Nếu email hợp lệ, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.";
        }

        // Mật khẩu mới
        public async Task<OperationResult> NewPassword(NewPasswordViewModel newPassVM)
        {
            // Giải mã mail
            string decodedEmail = Uri.UnescapeDataString(newPassVM.Email);

            // Kiểm tra account
            var user = await _userManager.FindByEmailAsync(decodedEmail);
            if (user == null)
            {
                return new OperationResult(false, "Không tìm thấy tài khoản !!!");
            }

            // Giải mã token
            string decodedToken = Uri.UnescapeDataString(newPassVM.Token);

            // Đặt lại mật khẩu mới
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassVM.Password);

            if (!result.Succeeded)
            {
                return new OperationResult(true, "Đã có lỗi vui lòng thử lại");
            }
            return new OperationResult(true, "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.");
        }

        // Tìm kiếm user từ email
        public async Task<UserViewModel> FindAccountByMailAsync(string userEmail)
        {
            // Kiểm tra user
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null) return null;

            return new UserViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        // Cập nhật thông tin tài khoản
        public async Task<OperationResult> UpdateAccountInfoAsync(UserViewModel userVM)
        {
            // Tìm user theo mail
            var user = await _userManager.FindByEmailAsync(userVM.Email);
            if (user == null)
                return new OperationResult(false, "Không tìm thấy thông tin tài khoản này !!!");

            // Lấy thông tin thay đổi
            user.Email = userVM.Email;
            user.UserName = userVM.Username;
            user.PhoneNumber = userVM.PhoneNumber;

            // Cập nhật
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new OperationResult(false, "Cập nhật thông tin tài khoản thất bại !!!");

            return new OperationResult(true, "Cập nhật thông tin tài khoản thành công !!!");
        }

        // Xử lý đăng nhập bằng Google
        // * Nếu user từng đăng nhập thì vào luôn, chưa thì tạo mới tài khoản liên kết rồi mới vào
        public async Task<OperationResult> HandleGoogleLoginAsync()
        {
            // B1: Lấy thông tin từ Google
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new OperationResult(false, "Không lấy được thông tin đăng nhập từ Google !!!");
            }

            // B2: Đăng nhập bằng Google - nếu user đã từng liên kết
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                // Gửi mail nếu vào thành công
                var mail = info.Principal.FindFirstValue(ClaimTypes.Email);
                SendLoginEmailAsync(mail);
                return new OperationResult(true, "Đăng nhập Google thành công !!!");
            }

            // B3: Nếu chưa từng liên kết trước đó - thì tạo mới
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (email == null)
            {
                return new OperationResult(false, "Không thể xác định email từ tài khoản Google !!!");
            }

            // Nếu email này chưa sử dụng thì tạo tài khoản
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new UserModel(name, email);
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new OperationResult(false, "Không thể tạo tài khoản từ Google !!!");
                }
            }

            // B4: Kiểm tra user đã liên kết với Google chưa
            var logins = await _userManager.GetLoginsAsync(user);
            var alreadyLinked = logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey);
            if (!alreadyLinked)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    return new OperationResult(false, "Không thể liên kết tài khoản Google !!!");
                }
            }

            // Đăng nhập
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Gửi mail nếu vào thành công
            SendLoginEmailAsync(email);

            return new OperationResult(true, "Đăng nhập với tài khoản Google thành công !!!");
        }

        // Gửi mail nếu đăng nhập thành công với tài khoản google
        private void SendLoginEmailAsync(string email)
        {
            var subject = "Đăng nhập thành công với tài khoản Google!";
            var body = "Chúc bạn có trải nghiệm vui vẻ nhé!";

            _ = Task.Run(async () =>
            {
                await _emailService.SendEmailAsync(email, subject, body);
            });
        }
    }
}
