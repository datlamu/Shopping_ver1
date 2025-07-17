using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // Danh sách đơn hàng
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách item
            var data = await _orderService.GetOrderlistAsync();

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(string orderCode, int status, int? page)
        {
            // Chi tiết đơn hàng
            var data = await _orderService.GetOrderDetailAsync(orderCode);

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            // Lưu orderCode
            ViewBag.OrderCode = orderCode;

            // Lưu trạng thái đơn hàng vào ViewBag
            ViewBag.OrderStatus = status;

            return View(data);
        }

        // Cập nhật đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            // Cập nhật và trả kết quả cho ajax
            var result = await _orderService.UpdateOrderAsync(orderCode, status);

            return Json(new
            {
                result.Success,
                result.Message
            });
        }

        // Xóa đơn hàng 
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa và kiểm tra
            var result = await _orderService.DeleteOrderAsync(id);

            return Json(new
            {
                result.Success,
                result.Message
            });
        }
    }
}
