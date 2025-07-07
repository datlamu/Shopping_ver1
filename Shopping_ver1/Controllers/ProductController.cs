using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Repository;

namespace Shopping_ver1.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            var product = await _dataContext.Products.ToListAsync();
            return View(product);
        }

        // Tìm kiếm sản phẩm
        [HttpGet]
        public async Task<IActionResult> Search(string? searchItem)
        {
            if (string.IsNullOrWhiteSpace(searchItem))
                return RedirectToAction("Index");

            var lowerSearch = searchItem.ToLower();

            var products = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p =>
                    p.Name.ToLower().Contains(lowerSearch) ||
                    p.Description.ToLower().Contains(lowerSearch) ||
                    p.Brand.Name.ToLower().Contains(lowerSearch) ||
                    p.Brand.Description.ToLower().Contains(lowerSearch) ||
                    p.Category.Name.ToLower().Contains(lowerSearch) ||
                    p.Category.Description.ToLower().Contains(lowerSearch)
                )
                .ToListAsync();

            ViewData["Keyword"] = searchItem;

            return View("Index", products);
        }

        // Chi tiết sản phẩm
        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var productById = await _dataContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            return View(productById);
        }
    }
}
