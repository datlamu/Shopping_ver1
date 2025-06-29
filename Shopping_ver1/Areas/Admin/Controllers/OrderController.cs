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
        public async Task<IActionResult> Index(int page = 1)
        {
            var (data, pager) = await _orderService.GetOrderlistAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(string orderCode, int status)
        {
            // Chi tiết đơn hàng
            var data = await _orderService.GetOrderDetailAsync(orderCode);

            ViewBag.OrderStatus = status;

            return View(data);
        }

        // Cập nhật đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            // Cập nhật và trả kết quả cho ajax
            var result = await _orderService.UpdateOrderAsync(orderCode, status);
            return Json(new { result.success, result.message });
        }

        // Xóa đơn hàng 
        public async Task<IActionResult> Delete(string orderCode)
        {
            // Xóa và kiểm tra
            var result = await _orderService.DeleteOrderAsync(orderCode);
            if (!result.success)
            {
                return NotFound();
            }

            // Xóa thành công
            TempData["Success"] = "Xóa danh đơn hàng thành công !!!";
            return RedirectToAction("Index");
        }
    }
}
