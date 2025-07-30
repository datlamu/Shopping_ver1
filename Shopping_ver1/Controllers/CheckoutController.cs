using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICouponService _couponService;
        private readonly ICartService _cartservice;

        public CheckoutController(ICheckoutService checkoutService, ICouponService couponService, ICartService cartservice)
        {
            _checkoutService = checkoutService;
            _couponService = couponService;
            _cartservice = cartservice;
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

            // Lấy phí vận chuyển
            var shippingCookie = Request.Cookies["ShippingInfo"];
            if (shippingCookie == null)
            {
                TempData["Error"] = "Bạn phải chọn đầy đủ thông tin để giao hàng!";
                return RedirectToAction("Index", "Cart");
            }
            var shipping = JsonConvert.DeserializeObject<ShippingModel>(shippingCookie);

            // Lấy mã giảm giá từ Session
            var appliedCouponCode = HttpContext.Session.GetString("CouponCode");
            CouponModel coupon = null;
            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                coupon = await _couponService.FindByCodeAsync(appliedCouponCode);
            }

            var listCartItem = _cartservice.GetListCartItem(shipping, coupon);
            if (!listCartItem.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Thực hiện thanh toán
            var result = await _checkoutService.CheckoutAsync(userEmail, listCartItem);
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
