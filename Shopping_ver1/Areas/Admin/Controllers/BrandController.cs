using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Ngăn không lưu cache
    public class BrandController : Controller
    {
        private readonly IBrandService _BrandService;

        public BrandController(IBrandService BrandService)
        {
            _BrandService = BrandService;
        }

        // Danh sách các thương hiệu
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách và phân trang
            var data = await _BrandService.GetlistItemAsync();

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Tạo thương hiệu mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new BrandModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel Brand)
        {
            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                return View(Brand);
            }

            // Thêm thương hiệu và kiểm tra
            var result = await _BrandService.CreateAsync(Brand);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(Brand);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Cập nhật thương hiệu
        [HttpGet]
        public async Task<IActionResult> Update(int id, int? page)
        {
            // Tìm thương hiệu đã chọn
            var Brand = await _BrandService.GetUpdateItemAsync(id);

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(Brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BrandModel Brand, int? page)
        {
            ViewBag.Page = page ?? 0;

            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                return View(Brand);
            }

            // Chỉnh sửa thương hiệu và kiểm tra
            var result = await _BrandService.UpdateAsync(Brand);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(Brand);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Xóa thương hiệu
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thương hiệu
            var result = await _BrandService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
