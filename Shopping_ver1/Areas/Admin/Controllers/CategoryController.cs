using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services;

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
        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _categoryService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }

        // Tạo thể loại mới
        [HttpGet]
        public IActionResult Create(int page = 1)
        {
            // Trang hiện tại
            ViewBag.Page = page;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category, int page = 1)
        {
            // Kiểm tra thông tin thể loại
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thể loại !!!";
                ViewBag.Page = page;
                return View(category);
            }

            // Thêm thể loại và kiểm tra
            var result = await _categoryService.CreateAsync(category);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.Page = page;
                return View(category);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page });
        }

        // Cập nhật thể loại
        [HttpGet]
        public async Task<IActionResult> Update(int id, int page = 1)
        {
            // Tìm thể loại đã chọn
            var category = await _categoryService.GetUpdateItemAsync(id);

            // Trang hiện tại
            ViewBag.Page = page;

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryModel category, int page = 1)
        {
            // Kiểm tra thông tin thể loại
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thể loại !!!";
                ViewBag.Page = page;
                return View(category);
            }

            // Chỉnh sửa thể loại và kiểm tra
            var result = await _categoryService.UpdateAsync(category);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.Page = page;
                return View(category);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page });
        }

        // Xóa thể loại
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thể loại
            var result = await _categoryService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }

        // Tải lại table cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetTable(int page = 1)
        {
            var (data, pager) = await _categoryService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_CategoryTablePartial", data);
        }
    }
}
