using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            // Tổng số Items
            var totalItems = await _dataContext.Orders.CountAsync();

            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page);

            // Danh sách items
            var data = await _dataContext.Orders
                .OrderByDescending(p => p.Id)
                .Skip(pager.Skip) // Bỏ qua số lượng phần tử
                .Take(pager.PageSize) // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            ViewBag.Pager = pager;

            return View(data);
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
