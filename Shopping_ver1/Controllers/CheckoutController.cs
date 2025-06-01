using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

namespace Shopping_ver1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _cks;
        private readonly IEmailService _emailService;
        public CheckoutController(ICheckoutService cks, IEmailService emailService)
        {
            _cks = cks;
            _emailService = emailService;
        }
        // Thực hiện thanh toán
        public async Task<IActionResult> Index()
        {
            // Kiểm tra xem user login chưa
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                TempData["Error"] = "Đăng nhập trước khi đặt hàng!";
                return RedirectToAction("Login", "Account");
            }

            // Lấy ra giỏ hàng và kiểm tra
            var carts = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            if (!carts.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Thực hiện thanh toán
            var orderCode = await _cks.CheckoutAsync(userEmail, carts);

            // Xóa giỏ hàng khỏi session
            HttpContext.Session.Remove("Cart");

            // Thông báo và quay lại trang giỏ hàng
            TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {orderCode}";
            return RedirectToAction("Index", "Cart");
        }
    }
}
