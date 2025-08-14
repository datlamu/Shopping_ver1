using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

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
            var product = await _productService.GetAllAsync();

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

            // Sản phẩm liên quan theo thể loại
            var relatedProducts = await _productService.RelatedByCategoryAsync(product.CategoryId, product.Id);

            // Đánh giá sản phẩm
            var reviewProducts = await _productService.GetReviewProduct(product.Id);

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.ReviewProducts = reviewProducts;

            return View(product);
        }

        // Đánh giá sản phẩm
        [HttpPost]
        public async Task<IActionResult> ProductReview(RatingModel rating)
        {
            // Kiểm tra thông tin nhập vào
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin đánh giá !!!";
                return View("Details", rating.ProductId);
            }

            // Gửi đánh giá và kiểm tra
            var result = await _productService.ItemReview(rating);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View("Details", rating.ProductId);
            }
            TempData["Success"] = result.Message;

            return Redirect(Request.Headers["Referer"]);
        }
    }
}
