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
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger,
            DataContext context,
            IContactService contactService,
            IBrandService brandService,
            IProductService productService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _dataContext = context;
            _contactService = contactService;
            _brandService = brandService;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string sort_by = "", int page = 1)
        {
            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .AsQueryable();

            // Lấy danh sách sản phẩm và phân trang ( có lọc )
            var (data, pager) = await _productService.GetPagedProductListAsync(productsAQ, sort_by, page);

            // ViewBag lưu tại phân trang và bộ lọc
            ViewBag.Pager = pager;
            ViewBag.Sort_by = sort_by;

            return View(data);
        }

        // Tải lại bảng sản phẩm
        public async Task<IActionResult> ProductPartial(string sort_by = "", int page = 1, string byAction = "", string slug = " ")
        {
            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .AsQueryable();

            if (byAction == "brand")
            {
                var brand = await _brandService.FindBySlugAsync(slug);
                if (brand == null) return RedirectToAction("Index");
                productsAQ = productsAQ.Where(p => p.BrandId == brand.Id);
            }
            else if (byAction == "category")
            {
                var category = await _categoryService.FindBySlugAsync(slug);
                if (category == null) return RedirectToAction("Index");
                productsAQ = productsAQ.Where(p => p.CategoryId == category.Id);
            }

            var (data, pager) = await _productService.GetPagedProductListAsync(productsAQ, sort_by, page);

            // ViewBag lưu tại phân trang
            ViewBag.Pager = pager;

            return PartialView("_ProductPartial", data);
        }

        // Trang liên hệ
        public async Task<IActionResult> Contact(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _contactService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return View(data);
        }

        // Tải lại bảng liên hệ 
        public async Task<IActionResult> ContactPartial(int page = 1)
        {
            var (data, pager) = await _contactService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_ContactPartial", data);
        }

        // Tìm sản phẩm theo thể loại
        public async Task<IActionResult> Categories(string slug = "", string sort_by = "", int page = 1)
        {
            // Tìm thương hiệu
            var category = await _categoryService.FindBySlugAsync(slug);
            if (category == null)
                return RedirectToAction("Index");

            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == category.Id)
                .AsQueryable();

            // Lấy danh sách sản phẩm và phân trang ( có lọc )
            var (data, pager) = await _productService.GetPagedProductListAsync(productsAQ, sort_by, page);

            // ViewBag lưu tại phân trang và bộ lọc cùng với hành động thực thi
            ViewBag.Pager = pager;
            ViewBag.Sort_by = sort_by;
            ViewBag.ByAction = "category";
            ViewBag.Slug = slug;

            return View("Index", data);
        }

        // Tìm sản phẩm theo thương hiệu
        public async Task<IActionResult> Brands(string slug = "", string sort_by = "", int page = 1)
        {
            // Tìm thương hiệu
            var brand = await _brandService.FindBySlugAsync(slug);
            if (brand == null)
                return RedirectToAction("Index");

            // Truy vấn danh sách sản phẩm
            var productsAQ = _dataContext.Products
                .Include(p => p.Inventory)
                .Where(p => p.BrandId == brand.Id)
                .AsQueryable();

            // Lấy danh sách sản phẩm và phân trang ( có lọc )
            var (data, pager) = await _productService.GetPagedProductListAsync(productsAQ, sort_by, page);

            // ViewBag lưu tại phân trang và bộ lọc cùng với hành động thực thi
            ViewBag.Pager = pager;
            ViewBag.Sort_by = sort_by;
            ViewBag.ByAction = "brand";
            ViewBag.Slug = slug;

            return View("Index", data);
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
