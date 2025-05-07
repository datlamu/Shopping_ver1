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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var productById = await _dataContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            return View(productById);
        }
    }
}
