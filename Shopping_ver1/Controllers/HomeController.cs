using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

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

        public async Task<IActionResult> Index()
        {
            var product = await _dataContext.Products.ToListAsync();
            return View(product);
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
