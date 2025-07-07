using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Ngăn không lưu cache
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Danh sách sản phẩm
        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _productService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }

        // Tạo sản phẩm mới
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy ra danh sách category và brand
            var (categories, brands) = await _productService.GetCategoryAndBrandListAsync();

            // Đưa danh sách vào ViewBag
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            return View(new ProductModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            // Lấy ra danh sách category và brand
            var (categories, brands) = await _productService.GetCategoryAndBrandListAsync(product.CategoryId, product.BrandId);

            // Đưa danh sách vào ViewBag
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            // Kiểm tra thông tin sản phẩm
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sản phẩm !!!";
                return View(product);
            }

            // Tạo sản phẩm mới và kiểm tra
            var result = await _productService.CreateAsync(product);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(product);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Chỉnh sửa sản phẩm
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Tìm sản phầm đã chọn
            var product = await _productService.FindProductsAsync(id);

            // Lấy ra danh sách category và brand
            var (categories, brands) = await _productService.GetCategoryAndBrandListAsync(product.CategoryId, product.BrandId);

            // Đưa danh sách vào ViewBag
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductModel product)
        {
            // Lấy ra danh sách category và brand
            var (categories, brands) = await _productService.GetCategoryAndBrandListAsync(product.CategoryId, product.BrandId);

            // Đưa danh sách vào ViewBag
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;

            // Kiểm tra thông tin sản phẩm
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin sản phẩm !!!";
                return View(product);
            }

            // Cập nhật và kiểm tra
            var result = await _productService.UpdateAsync(product);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(product);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            // Sản phẩm
            var result = await _productService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }

        // Tải lại table cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetTable(int page = 1)
        {
            var (data, pager) = await _productService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_ProductTablePartial", data);
        }
    }
}
