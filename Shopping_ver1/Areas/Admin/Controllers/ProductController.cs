using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IProductService _productService;
        public ProductController(DataContext context, IProductService productService)
        {
            _dataContext = context;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var product = await _dataContext.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();

            return View(product);
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
            // Kiểm tra thông tin sản phẩm
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sản phẩm !!!";
                return View(product);
            }

            // Lấy ra slug dựa vào tên sản phẩm
            product.Slug = _productService.GenerateSlug(product.Name);

            // Kiểm tra xem sản phẩm này tồn tại chưa
            if (!await _productService.IsSlugUniqueAsync(product.Slug))
            {
                ModelState.AddModelError("", "Sản phẩm đã tồn tại");
                return View(product);
            }

            // Chắc chắn người dùng đã chọn file
            // Lý do cần kiểm ra dù đã có ModelState.IsValid là để tránh các file rác và lỗi 
            if (product.ImageUpload == null || product.ImageUpload.Length == 0)
            {
                ModelState.AddModelError("ImageUpload", "Vui lòng chọn ảnh sản phẩm");
                return View(product);
            }

            // Lưu ảnh và và tạo mới sản phẩm
            var imageName = await _productService.SaveImageAsync(product.ImageUpload);
            await _productService.SaveProductAsync(product, imageName);

            TempData["Success"] = "Thêm sản phẩm thành công!!!";
            return RedirectToAction("Index");
        }
    }
}
