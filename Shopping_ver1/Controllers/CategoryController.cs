using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;

namespace Shopping_ver1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string slug = "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == slug).FirstOrDefault();
            if (category == null) return RedirectToAction("Index");
            var productByCategory = await _dataContext.Products.Where(p => p.CategoryId == category.Id).OrderByDescending(p => p.Id).ToListAsync();
            return View(productByCategory);
        }
    }
}
