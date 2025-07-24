using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly DataContext _dataContext;

        public UserController(RoleManager<IdentityRole> roleManager, IUserService userService, DataContext dataContext)
        {
            _roleManager = roleManager;
            _userService = userService;
            _dataContext = dataContext;
        }
        // Danh sách các user
        public async Task<IActionResult> Index(int? page)
        {
            // Danh sách items - Linq
            var data = await (
                from u in _dataContext.Users
                join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                join r in _dataContext.Roles on ur.RoleId equals r.Id
                select new
                {
                    User = u,
                    RoleName = r.Name
                }
            )
            .ToListAsync();

            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Tạo user mới
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách role
            var roles = await _roleManager.Roles.ToListAsync();

            // Đưa danh sách role và ViewModel và role mặc định là user
            var user = new RegisterViewModel
            {
                RoleId = roles.FirstOrDefault(r => r.Name == "User").Id,
                Roles = await GetRolesAsync()
            };

            return View(user);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel user)
        {
            // Kiểm tra thông tin
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng kiểm tra lại thông tin !!!");
                user.Roles = await GetRolesAsync();
                return View(user);
            }

            // Lấy ra Role từ roleId
            var role = await _roleManager.FindByIdAsync(user.RoleId);

            // Đăng ký tài khoản
            var result = await _userService.RegisterAsync(user, role.Name);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Đăng ký thất bại, vui lòng xem lại thông tin đăng ký");
                user.Roles = await GetRolesAsync();
                return View(user);
            }

            // Đăng ký thành công
            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Index");
        }
        // Chỉnh sửa thông tin user
        [HttpGet]
        public async Task<IActionResult> Update(string id, int? page)
        {
            var user = await _userService.GetEditUserViewModelAsync(id);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người dùng này!";
                return NotFound();
            }

            ViewBag.Page = page ?? 0;

            return View(user);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(EditUserViewModel editUser, int? page)
        {
            ViewBag.Page = page ?? 0;

            // Kiểm tra dữ liệu
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng kiểm tra lại thông tin !!!");
                return View(editUser);
            }

            // Thực hiện chỉnh sửa
            var result = await _userService.EditUserAsync(editUser);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(editUser);
            }

            // Chỉnh sửa thành công
            TempData["Success"] = result.Message;
            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Lấy roles
        private async Task<SelectList> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return new SelectList(roles, "Id", "Name");
        }

        // Xóa user
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
