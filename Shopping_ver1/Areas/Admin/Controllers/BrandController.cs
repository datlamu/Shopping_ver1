using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IBrandService _bs;

        public BrandController(DataContext context, IBrandService bs)
        {
            _dataContext = context;
            _bs = bs;
        }
        public async Task<IActionResult> Index()
        {
            var brands = await _dataContext.Brands
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(brands);
        }
        // Tạo sản danh thương hiệu
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                return View(brand);
            }

            // Lấy ra slug dựa vào tên thương hiệu
            brand.Slug = _bs.GenerateSlug(brand.Name);

            // Kiểm tra xem thương hiệu này tồn tại chưa
            if (!await _bs.IsSlugUnique(brand.Slug))
            {
                TempData["Error"] = "Thương hiệu này đã tồn tại !!!";
                return View(brand);
            }

            // Lưu lại thương hiệu
            await _bs.SaveBrand(brand, "Create");

            TempData["Success"] = "Thêm thương hiệu thành công!!!";
            return RedirectToAction("Index");
        }

        // Chỉnh sửa thương hiệu
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm thương hiệu đã chọn
            var brand = await _dataContext.Brands.FindAsync(id);

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View("Create", brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            // Kiểm tra thông tin thương hiệu
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin thương hiệu !!!";
                return View("Create", brand);
            }

            // Lấy ra slug dựa vào tên thương hiệu
            brand.Slug = _bs.GenerateSlug(brand.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Brands
                .AsNoTracking()
                .Where(c => c.Id == brand.Id)
                .Select(c => c.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem thương hiệu này tồn tại chưa
            if (brand.Slug != oldSlug && !await _bs.IsSlugUnique(brand.Slug))
            {
                TempData["Error"] = "Thương hiệu đã tồn tại !!!";
                return View("Create", brand);
            }

            // Cập nhật mới thương hiệu
            await _bs.SaveBrand(brand, "Edit");

            TempData["Success"] = "Chỉnh sửa thương hiệu thành công!!!";
            return RedirectToAction("Index");
        }

        // Xóa thương hiệu
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa thương hiệu
            if (await _bs.DeleteBrand(id))
                TempData["Success"] = "Xóa thương hiệu thành công!!!";
            else
                TempData["Error"] = "Đã có lỗi khi xóa";
            return RedirectToAction("Index");
        }
    }
}
