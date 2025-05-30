using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IProductService _ps;
        public ProductController(DataContext context, IProductService productService)
        {
            _dataContext = context;
            _ps = productService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            // Tổng số Items
            var totalItems = await _dataContext.Products.CountAsync();

            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page);

            // Danh sách items
            var data = await _dataContext.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Skip(pager.Skip) // Bỏ qua số lượng phần tử
                .Take(pager.PageSize) // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            ViewBag.Pager = pager;

            return View(data);
        }

        // Tạo sản phẩm mới
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy ra danh sách category và brand
            var categories = await _dataContext.Categories.ToListAsync();
            var brands = await _dataContext.Brands.ToListAsync();

            // Đưa danh sách vào VewBag với ( value = Id, text = Name)
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Brands = new SelectList(brands, "Id", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            // Lấy ra danh sách category và brand
            var categories = await _dataContext.Categories.ToListAsync();
            var brands = await _dataContext.Brands.ToListAsync();

            // Đưa danh sách vào VewBag với ( value = Id, text = Name)
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(brands, "Id", "Name", product.BrandId);

            // Kiểm tra thông tin sản phẩm
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sản phẩm !!!";
                return View(product);
            }

            // Lấy ra slug dựa vào tên sản phẩm
            product.Slug = _ps.GenerateSlug(product.Name);

            // Kiểm tra xem sản phẩm này tồn tại chưa
            if (!await _ps.IsSlugUnique(product.Slug))
            {
                TempData["Error"] = "Sản phẩm này đã tồn tại !!!";
                return View(product);
            }

            // Chắc chắn chắn chọn ảnh và đảm bảo không phải ảnh rác hoặc lỗi 
            if (product.ImageUpload == null || product.ImageUpload.Length == 0)
            {
                ModelState.AddModelError("ImageUpload", "Vui lòng chọn ảnh sản phẩm");
                return View(product);
            }

            // Lưu ảnh và và tạo mới sản phẩm
            var imageName = await _ps.SaveImage(product.ImageUpload);
            await _ps.SaveProduct(product, imageName, "Create");

            TempData["Success"] = "Thêm sản phẩm thành công!!!";
            return RedirectToAction("Index");
        }

        // Chỉnh sửa sản phẩm
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm sản phầm đã chọn
            var product = await _dataContext.Products.FindAsync(id);

            // Lấy ra danh sách category và brand
            var categories = await _dataContext.Categories.ToListAsync();
            var brands = await _dataContext.Brands.ToListAsync();

            // Đưa danh sách vào vewbag và chọn ra item defaul dựa theo dữ liệu trước đó
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(brands, "Id", "Name", product.BrandId);

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View("Create", product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            // Kiểm tra thông tin sản phẩm
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sản phẩm !!!";
                return View("Create", product);
            }

            // Lấy ra slug dựa vào tên sản phẩm
            product.Slug = _ps.GenerateSlug(product.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Products
                .AsNoTracking()
                .Where(p => p.Id == product.Id)
                .Select(p => p.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem sản phẩm này tồn tại chưa
            if (product.Slug != oldSlug && !await _ps.IsSlugUnique(product.Slug))
            {
                TempData["Error"] = "Sản phẩm đã tồn tại !!!";
                return View("Create", product);
            }

            // Kiểm tra trường hợp upload ảnh mới nhưng là ảnh rác hoặc lỗi 
            if (product.ImageUpload != null && product.ImageUpload.Length == 0)
            {
                ModelState.AddModelError("ImageUpload", "Ảnh bị lỗi");
                return View("Create", product);
            }

            // Cập nhật mới sản phẩm
            string imageName = product.Image;
            if (product.ImageUpload != null)
                imageName = await _ps.SaveImage(product.ImageUpload);
            await _ps.SaveProduct(product, imageName, "Edit");

            TempData["Success"] = "Chỉnh sửa sản phẩm thành công!!!";
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa sản phẩm
            if (await _ps.DeleteProduct(id))
                TempData["Success"] = "Xóa sản phẩm thành công!!!";
            else
                TempData["Erorr"] = "Đã có lỗi khi xóa";
            return RedirectToAction("Index");
        }
    }
}
