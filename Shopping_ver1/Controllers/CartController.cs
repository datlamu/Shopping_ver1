using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_ver1.Migrations;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;
using static Azure.Core.HttpHeader;
namespace Shopping_ver1.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartservice;
        private readonly ICouponService _couponService;
        private readonly IShippingService _shippingService;
        public CartController(ICartService cartservice, ICouponService couponService, IShippingService shippingService)
        {
            _cartservice = cartservice;
            _couponService = couponService;
            _shippingService = shippingService;
        }

        // Hiện sản phẩm trong giỏ hàng
        public IActionResult Index()
        {
            var shippingCookie = Request.Cookies["ShippingInfo"];

            ShippingModel shipping;
            if (shippingCookie != null)
                shipping = JsonConvert.DeserializeObject<ShippingModel>(shippingCookie);
            else
                shipping = new ShippingModel();

            ViewBag.Shipping = shipping;

            var listCartItem = _cartservice.GetListCartItem();

            return View(listCartItem);
        }

        // Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            var result = await _cartservice.AddAsync(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Tăng số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Increase(int id)
        {
            var result = await _cartservice.Increase(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Giảm số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult Decrease(int id)
        {
            var result = _cartservice.Decrease(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Cập nhật số lượng giỏ hàng
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var result = _cartservice.UpdateQuantityCart(productId, quantity);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Xóa sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult Remove(int id)
        {
            var result = _cartservice.Remove(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Xóa toàn bộ sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult Clear()
        {
            var result = _cartservice.Clear();

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        // Tải lại table giỏ hàng để cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetCartTable()
        {
            // Lấy phí ship từ cookie và kiểm tra
            var shippingCookie = Request.Cookies["ShippingInfo"];
            ShippingModel shipping = null;
            if (shippingCookie != null)
            {
                shipping = JsonConvert.DeserializeObject<ShippingModel>(shippingCookie);
            }

            // Lấy mã giảm giá từ Session
            var appliedCouponCode = HttpContext.Session.GetString("CouponCode");
            CouponModel coupon = null;
            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                coupon = await _couponService.FindByCodeAsync(appliedCouponCode);
            }

            // Lấy danh sách sản phẩm trong giỏ hàng
            var listCartItem = _cartservice.GetListCartItem(shipping, coupon);

            return PartialView("_CartTablePartial", listCartItem);
        }

        // Tính phí ship
        public async Task<IActionResult> GetShipping(string city, string district, string ward)
        {
            // Lấy shipping
            var shipping = await _shippingService.GetShippingAsync(city, district, ward);

            // Set Cookie
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Secure = true
            };

            // object thành JSON
            var jsonShipping = JsonConvert.SerializeObject(shipping);

            // Thêm Cookie
            Response.Cookies.Append("ShippingInfo", jsonShipping, options);

            return Json(new { success = true, message = "tính phí ship thành công" });
        }

        // Kích hoạt coupon
        public async Task<IActionResult> ActiveCoupon(string couponCode, decimal totalPrice)
        {
            // Tìm Coupon theo code
            var coupon = await _couponService.FindByCodeAsync(couponCode);

            // Kiểm tra tính hợp lệ
            var result = _couponService.ValidateCoupon(coupon, totalPrice);
            if (result.Success)
            {
                // Lưu vào Session
                HttpContext.Session.SetString("CouponCode", coupon.Code);
            }

            return Json(new { result.Success, result.Message });
        }
    }
}
