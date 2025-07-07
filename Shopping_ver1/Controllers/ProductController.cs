using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Services;

namespace Shopping_ver1.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var product = await _productService.GetlistItemAsync();

            return View(product);
        }

        // Tìm kiếm sản phẩm
        [HttpGet]
        public async Task<IActionResult> Search(string? searchItem)
        {
            // Nếu trống thì trả về trang cũ
            if (string.IsNullOrWhiteSpace(searchItem))
                return RedirectToAction("Index");

            // Tìm kiếm các sản phẩm
            var products = await _productService.SearchItem(searchItem);

            // Lưu lại từ khóa để gán lại cho trang sau
            ViewData["Keyword"] = searchItem;

            return View("Index", products);
        }

        // Chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra id
            if (id == null)
                return RedirectToAction("Index");

            // Tìm sản phẩm và kiểm tra
            var product = await _productService.FindProductsAsync(id.Value);
            if (product == null)
                return NotFound();

            // Tìm sản phẩm liên quan theo thể loại
            var relatedProducts = await _productService.relatedItemsAsync(product.CategoryId, product.Id);

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}
