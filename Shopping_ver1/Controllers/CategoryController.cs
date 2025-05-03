using Microsoft.AspNetCore.Mvc;

namespace Shopping_ver1.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
