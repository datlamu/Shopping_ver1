using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Services.Abstract;

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
        public async Task<IActionResult> OrderDetail(string orderCode, int? page)
        {
            // Kiểm tra OrderCode
            if (string.IsNullOrWhiteSpace(orderCode))
            {
                return NotFound("Mã đơn hàng không hợp lệ..");
            }

            // Chi tiết đơn hàng
            var orderDetailVM = await _orderService.GetOrderDetailAsync(orderCode);

            // Kiểm tra ViewModel có dữ liệu hay không
            if (orderDetailVM == null || orderDetailVM.OrderDetail == null || !orderDetailVM.OrderDetail.Any())
            {
                // Có thể trả về trang thông báo hoặc redirect về danh sách đơn hàng
                return NotFound("Không tìm thấy thông tin đơn hàng.");
            }

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(orderDetailVM);
        }

        // Cập nhật đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string orderCode, int status)
        {
            // Cập nhật và trả kết quả cho ajax
            var result = await _orderService.UpdateOrderAsync(orderCode, status);

            return Json(new { result.Success, result.Message });
        }

        // Xóa đơn hàng 
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa và kiểm tra
            var result = await _orderService.DeleteOrderAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
