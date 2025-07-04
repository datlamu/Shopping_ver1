using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // Danh sách các thương hiệu
        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _brandService.GetBrandlistAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }

        // Tạo thương hiệu mới
        [HttpGet]
        public IActionResult Create(int page = 1)
        {
            // Trang hiện tại
            ViewBag.Page = page;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand, int page = 1)
        {
            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                ViewBag.Page = page;
                return View(brand);
            }

            // Thêm thương hiệu và kiểm tra
            var result = await _brandService.CreateBrandAsync(brand);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.Page = page;
                return View(brand);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page });
        }

        // Cập nhật thương hiệu
        [HttpGet]
        public async Task<IActionResult> Update(int id, int page = 1)
        {
            // Tìm thương hiệu đã chọn
            var brand = await _brandService.GetUpdateBrandAsync(id);

            // Trang hiện tại
            ViewBag.Page = page;

            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BrandModel brand, int page = 1)
        {
            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                ViewBag.Page = page;
                return View(brand);
            }

            // Chỉnh sửa thương hiệu và kiểm tra
            var result = await _brandService.UpdateBrandAsync(brand);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.Page = page;
                return View(brand);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page });
        }

        // Xóa thương hiệu
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thương hiệu
            var result = await _brandService.DeleteBrandAsync(id);

            return Json(new { result.Success, result.Message });
        }

        // Tải lại table cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetTable(int page = 1)
        {
            var (data, pager) = await _brandService.GetBrandlistAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_BrandTablePartial", data);
        }
    }
}
