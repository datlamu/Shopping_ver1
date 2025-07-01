using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Services;

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
            // Lấy sản phẩm từ giỏ hàng
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
        public IActionResult Increase(int id)
        {
            var result = _cartservice.Increase(id);

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
            var model = _cartservice.GetListCartItem();
            return PartialView("_CartTablePartial", model);
        }
    }
}
