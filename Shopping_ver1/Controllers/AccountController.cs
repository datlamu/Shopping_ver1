using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Controllers
{
    public class AccountController : Controller
    {
        // Service
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        // Inject Service
        public AccountController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // Đăng nhập tài khoản
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            // Kiểm tra dữ liệu
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng kiểm tra lại thông tin !!!");
                return View(user);
            }

            // Kết quả đăng nhập
            var result = await _userService.LoginAsync(user);

            // Đăng nhập thất bại
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản hoặc Mật khẩu bị sai");
                return View(user);
            }

            TempData["Success"] = "Đăng nhập thành công!";
            return Redirect(user.ReturnUrl ?? "/");
        }

        // Đăng ký tài khoản
        [HttpGet]
        public ActionResult Register()
        {
            var user = new RegisterViewModel();
            return View(user);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            // Kiểm tra thông tin
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng kiểm tra lại thông tin !!!");
                return View(user);
            }

            // Đăng ký tài khoản
            var result = await _userService.RegisterAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Đăng ký thất bại, vui lòng xem lại thông tin đăng ký");
                return View(user);
            }

            // Đăng ký thành công
            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("Login");
        }

        // Đăng xuất
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _userService.LogoutAsync();
            TempData["Success"] = "Đăng xuất thành công!";
            return Redirect(returnUrl);
        }

        // Lịch sử đơn hàng
        [Authorize]
        public async Task<IActionResult> HistoryOrder()
        {
            // Lấy email người dùng
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Lấy danh sách đơn hàng theo email
            var orders = await _orderService.FindByUserEmailAsync(userEmail);

            if (orders == null || !orders.Any())
            {
                TempData["Message"] = "Bạn chưa có đơn hàng nào.";
                return View(new List<OrderModel>());
            }

            return View(orders);
        }

        // Hủy đơn hàng
        public async Task<IActionResult> CancelOrder(int id)
        {
            // Hủy đơn hàng theo id
            var result = await _orderService.DeleteOrderAsync(id);

            return Json(new { success = result.Success, message = result.Message });
        }

        // Tải lại bảng 
        [Authorize]
        public async Task<IActionResult> ReloadTable()
        {
            // Lấy email người dùng
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Lấy danh sách đơn hàng theo email
            var orders = await _orderService.FindByUserEmailAsync(userEmail);

            return PartialView("_HistoryOrderTablePartial", orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(string orderCode)
        {
            // Lấy danh sách đơn hàng theo email
            var orderDetailVM = await _orderService.GetOrderDetailAsync(orderCode);

            return View(orderDetailVM);
        }

        // Quên mật khẩu
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> ForgotPassword(UserModel user)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var message = await _userService.ForgotPassword(user.Email, baseUrl);

            TempData["Success"] = message;
            return RedirectToAction("Login");
        }

        // Mật khẩu mới
        [HttpGet]
        public ActionResult NewPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login"); // hoặc View lỗi
            }

            var model = new NewPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> NewPassword(NewPasswordViewModel newPassVM)
        {
            if (!ModelState.IsValid)
            {
                return View(newPassVM);
            }

            var result = await _userService.NewPassword(newPassVM);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Login");
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }

        // Mật khẩu mới
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AccountInfo()
        {
            // Lấy email người dùng
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Nếu email rỗng
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound();
            }

            var account = await _userService.FindAccountByMailAsync(userEmail);

            return View(account);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> AccountInfo(UserViewModel userVM)
        {
            if (!ModelState.IsValid)
            {
                return View(userVM);
            }

            var result = await _userService.UpdateAccountInfoAsync(userVM);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(userVM);
            }

            TempData["Success"] = result.Message;
            return View(userVM);
        }
    }
}
