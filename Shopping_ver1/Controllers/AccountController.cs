using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Services;

namespace Shopping_ver1.Controllers
{
    public class AccountController : Controller
    {
        // Service
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        // Inject Service
        public AccountController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
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
            // Kiểm tra dữ liệu
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng kiểm tra lại thông tin !!!");
                return View(user);
            }

            // Kết quả đăng ký
            var result = await _userService.RegisterAsync(user);

            // Đăng ký thất bại
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

    }
}
