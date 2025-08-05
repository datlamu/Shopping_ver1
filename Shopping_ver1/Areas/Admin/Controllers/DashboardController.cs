using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // Thống kê doanh thu
        public async Task<IActionResult> Index()
        {
            var data = await _dashboardService.GetDashboardStatsAsync();

            return View(data);
        }
    }
}
