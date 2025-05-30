using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICategoryService _cs;

        public CategoryController(DataContext context, ICategoryService cs)
        {
            _dataContext = context;
            _cs = cs;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            // Tổng số Items
            var totalItems = await _dataContext.Categories.CountAsync();

            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page);

            // Danh sách items
            var data = await _dataContext.Categories
                .OrderByDescending(p => p.Id)
                .Skip(pager.Skip) // Bỏ qua số lượng phần tử
                .Take(pager.PageSize) // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            ViewBag.Pager = pager;

            return View(data);
        }
        // Tạo sản danh mục mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            // Kiểm tra thông tin danh mục
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin danh mục !!!";
                return View(category);
            }

            // Lấy ra slug dựa vào tên danh mục
            category.Slug = _cs.GenerateSlug(category.Name);

            // Kiểm tra xem danh mục này tồn tại chưa
            if (!await _cs.IsSlugUnique(category.Slug))
            {
                TempData["Error"] = "Danh mục này đã tồn tại !!!";
                return View(category);
            }

            // Lưu lại danh mục
            await _cs.SaveCategory(category, "Create");

            TempData["Success"] = "Thêm danh mục thành công!!!";
            return RedirectToAction("Index");
        }

        // Chỉnh sửa danh mục
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm danh mục đã chọn
            var category = await _dataContext.Categories.FindAsync(id);

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View("Create", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            // Kiểm tra thông tin danh mục
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin danh mục !!!";
                return View("Create", category);
            }

            // Lấy ra slug dựa vào tên danh mục
            category.Slug = _cs.GenerateSlug(category.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Categories
                .AsNoTracking()
                .Where(c => c.Id == category.Id)
                .Select(c => c.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem danh mục này tồn tại chưa
            if (category.Slug != oldSlug && !await _cs.IsSlugUnique(category.Slug))
            {
                TempData["Error"] = "danh mục đã tồn tại !!!";
                return View("Create", category);
            }

            // Cập nhật mới danh mục
            await _cs.SaveCategory(category, "Edit");

            TempData["Success"] = "Chỉnh sửa danh mục thành công!!!";
            return RedirectToAction("Index");
        }

        // Xóa danh mục
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa danh mục
            if (await _cs.DeleteCategory(id))
                TempData["Success"] = "Xóa danh mục thành công!!!";
            else
                TempData["Error"] = "Đã có lỗi khi xóa";
            return RedirectToAction("Index");
        }
    }
}
