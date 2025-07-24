using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
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
            var result = await _checkoutService.CheckoutAsync(userEmail, carts);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Cart");
            }

            // Xóa giỏ hàng khỏi session
            HttpContext.Session.Remove("Cart");

            // Đặt hàng thành công
            TempData["Success"] = result.Message;
            return RedirectToAction("Index", "Cart");
        }
    }
}
