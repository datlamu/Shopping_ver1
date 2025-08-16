using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICouponService _couponService;
        private readonly ICartService _cartservice;
        private readonly IOrderService _orderService;

        public CheckoutController(
            ICheckoutService checkoutService,
            ICouponService couponService,
            ICartService cartservice,
            IOrderService orderService)
        {
            _checkoutService = checkoutService;
            _couponService = couponService;
            _cartservice = cartservice;
            _orderService = orderService;
        }
        //Thực hiện thanh toán
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

            // Lấy sản phẩm và tính luôn phí ship cùng coupon cho đơn hàng
            var listCartItem = _cartservice.GetAll(shipping, coupon);
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

        // Thanh toán với MoMo
        public async Task<IActionResult> PayWithMoMo()
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

            // Lấy sản phẩm và tính luôn phí ship cùng coupon cho đơn hàng
            var cartVM = _cartservice.GetAll(shipping, coupon);
            if (!cartVM.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Lưu tạm đơn hàng
            var (result, order) = await _checkoutService.CreatePendingOrderAsync(userEmail, cartVM);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Cart");
            }

            // Tạo link thanh toán MoMo
            var payUrl = await _checkoutService.CreatePaymentMoMoAsync(
                amount: (int)order.TotalPayment,
                orderInfo: $"Thanh toán đơn hàng {order.OrderCode}",
                orderCode: order.OrderCode
            );

            // test data khi tạo mã quét
            //return Json(payUrl);
            return Redirect(payUrl);
        }

        // ReturnUrl từ MoMo
        public IActionResult PaymentCallBack(int resultCode)
        {
            // Nếu thành công trả về lịch sử đơn hàng
            if (resultCode == 0)
            {
                TempData["Success"] = "Thanh toán đơn hàng thành công !!!";
                return RedirectToAction("HistoryOrder", "Account");
            }
            // Hủy đơn về trang giỏ hàng
            TempData["Error"] = "Hủy thanh toán thành công !!!";
            return RedirectToAction("Index", "Cart");
        }

        // NotifyUrl từ MoMo - xử lý chính xác trạng thái đơn hàng
        [AllowAnonymous]
        public async Task<IActionResult> MomoNotify([FromBody] MoMoNotifyModel notifyData)
        {
            string orderCode = notifyData.orderId;
            int resultCode = notifyData.resultCode;

            if (resultCode == 0)
            {
                // Xác nhận đơn đã thanh toán thành công
                await _checkoutService.ConfirmOrderAsync(orderCode);
            }
            else
            {
                // Hủy đơn nếu thanh toán thất bại
                await _checkoutService.CancelOrderAsync(orderCode);
            }

            // MoMo yêu cầu trả lại HTTP 200 OK
            return Ok(new { message = "Received" });
        }
    }
}
