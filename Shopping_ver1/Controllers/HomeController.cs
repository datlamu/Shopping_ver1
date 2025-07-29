using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IContactService _contactService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context, IContactService contactService)
        {
            _logger = logger;
            _dataContext = context;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index(string sort_by = "")
        {
            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .AsQueryable();

            // Lọc sản phẩm
            productsAQ = SortProducts(productsAQ, sort_by);

            // Lấy sản phẩm từ database
            var products = await productsAQ.ToListAsync();

            return View("Index", products);

        }

        public async Task<IActionResult> Contact(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _contactService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }
        // Tải lại table cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> ContactPartial(int page = 1)
        {
            var (data, pager) = await _contactService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_ContactPartial", data);
        }

        // Tìm sản phẩm theo thể loại
        public async Task<IActionResult> Categories(string slug = "", string sort_by = "")
        {
            // Tìm thể loại
            var category = _dataContext.Categories.FirstOrDefault(c => c.Slug == slug);
            if (category == null) return RedirectToAction("Index");

            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == category.Id)
                .AsQueryable();

            // Lọc sản phẩm
            productsAQ = SortProducts(productsAQ, sort_by);

            // Lấy sản phẩm từ database
            var products = await productsAQ.ToListAsync();

            return View("Index", products);
        }

        // Tìm sản phẩm theo thương hiệu
        public async Task<IActionResult> Brands(string slug = "", string sort_by = "")
        {
            // Tìm thương hiệu
            var brand = _dataContext.Brands.FirstOrDefault(c => c.Slug == slug);
            if (brand == null)
                return RedirectToAction("Index");

            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .Where(p => p.BrandId == brand.Id)
                .AsQueryable();

            // Lọc sản phẩm
            productsAQ = SortProducts(productsAQ, sort_by);

            // Lấy sản phẩm từ database
            var products = await productsAQ.ToListAsync();

            return View("Index", products);
        }

        // Lọc sản phẩm
        private IQueryable<ProductModel> SortProducts(IQueryable<ProductModel> productsAQ, string sort_by)
        {
            switch (sort_by)
            {
                case "price_increase":
                    productsAQ = productsAQ.OrderBy(p => p.Price);
                    break;
                case "price_decrease":
                    productsAQ = productsAQ.OrderByDescending(p => p.Price);
                    break;
                case "newest":
                    productsAQ = productsAQ.OrderByDescending(p => p.Id);
                    break;
                case "oldest":
                    productsAQ = productsAQ.OrderBy(p => p.Id);
                    break;
                default:
                    break;
            }
            return productsAQ;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
                return View("NotFound");
            else
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
