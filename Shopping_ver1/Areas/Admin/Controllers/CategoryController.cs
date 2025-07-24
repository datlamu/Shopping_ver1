using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Danh sách các thể loại
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách và phân trang
            var data = await _categoryService.GetlistItemAsync();

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Tạo thể loại mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new BrandModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            // Kiểm tra thông tin thể loại
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thể loại !!!";
                return View(category);
            }

            // Thêm thể loại và kiểm tra
            var result = await _categoryService.CreateAsync(category);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(category);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Cập nhật thể loại
        [HttpGet]
        public async Task<IActionResult> Update(int id, int? page)
        {
            // Tìm thể loại đã chọn
            var category = await _categoryService.GetUpdateItemAsync(id);

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryModel category, int? page)
        {
            ViewBag.Page = page ?? 0;

            // Kiểm tra thông tin thể loại
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thể loại !!!";
                return View(category);
            }

            // Chỉnh sửa thể loại và kiểm tra
            var result = await _categoryService.UpdateAsync(category);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(category);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Xóa thể loại
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thể loại
            var result = await _categoryService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
