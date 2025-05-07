using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;

namespace Shopping_ver1.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == slug).FirstOrDefault();
            if (brand == null) return RedirectToAction("Index");
            var productByBrand = await _dataContext.Products.Where(p => p.BrandId == brand.Id).OrderByDescending(p => p.Id).ToListAsync();
            return View(productByBrand);
        }
    }
}
