using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Repository;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        // Danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            // Lấy ra danh sách đơn hàng
            var orders = await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync();

            return View(orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(string orderCode)
        {
            // Lấy ra chi tiết của đơn hàng đó dựa trên orderCode
            var orderDetails = await _dataContext.OrderDetails
                .Include(o => o.Product)
                .Where(od => od.OrderCode == orderCode)
                .ToListAsync();

            return View(orderDetails);
        }
    }
}
