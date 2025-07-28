using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_ver1.Migrations;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;
namespace Shopping_ver1.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartservice;
        public CartController(ICartService cartservice)
        {
            _cartservice = cartservice;
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
        public IActionResult GetCartTable()
        {
            var shippingCookie = Request.Cookies["ShippingInfo"];

            ShippingModel shipping;

            if (shippingCookie != null)
                shipping = JsonConvert.DeserializeObject<ShippingModel>(shippingCookie);
            else
                shipping = new ShippingModel();

            ViewBag.Shipping = shipping;

            var listCartItem = _cartservice.GetListCartItem();

            return PartialView("_CartTablePartial", listCartItem);
        }

        // Tải lại table giỏ hàng để cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetShipping(string city, string district, string ward)
        {
            var shipping = await _cartservice.GetShippingAsync(city, district, ward);

            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Secure = true
            };

            // Serialize object thành JSON string
            var jsonShipping = JsonConvert.SerializeObject(shipping);

            Response.Cookies.Append("ShippingInfo", jsonShipping, options);

            return Json(new { success = true, message = "tính phí ship thành công" });
        }
    }
}
