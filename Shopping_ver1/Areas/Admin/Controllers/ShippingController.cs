using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Ngăn không lưu cache
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IShippingService _shippingService;
        public ShippingController(DataContext dataContext, IShippingService shippingService)
        {
            _dataContext = dataContext;
            _shippingService = shippingService;
        }

        public async Task<IActionResult> Index()
        {
            var shipping = await _shippingService.GetlistItemAsync();
            return View(shipping);
        }

        // Danh sách các thương hiệu
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(decimal price, string city, string district, string ward)
        {

            var shipping = new ShippingModel(price, city, district, ward);

            var result = await _shippingService.CreateAsync(shipping);

            return Json(new { result.Success, result.Message });
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, decimal newPrice)
        {
            // Cập nhật và trả kết quả cho ajax
            var result = await _shippingService.UpdateAsync(id, newPrice);

            return Json(new
            {
                result.Success,
                result.Message
            });
        }

        // Xóa thể loại
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thể loại
            var result = await _shippingService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
