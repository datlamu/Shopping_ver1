using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Ngăn không lưu cache
    public class SliderController : Controller
    {
        private readonly ISliderService _SliderService;
        public SliderController(ISliderService SliderService)
        {
            _SliderService = SliderService;
        }

        // Danh sách Slider
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách và phân trang
            var data = await _SliderService.GetlistItemAsync();

            ViewData["Page"] = page ?? 0;

            return View(data);
        }

        // Tạo Slider mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SliderModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel Slider)
        {
            // Kiểm tra thông tin Slider
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin Slider !!!";
                return View(Slider);
            }

            // Tạo Slider mới và kiểm tra
            var result = await _SliderService.CreateAsync(Slider);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(Slider);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Chỉnh sửa Slider
        [HttpGet]
        public async Task<IActionResult> Update(int id, int? page)
        {
            // Tìm Slider đã chọn
            var Slider = await _SliderService.FindItemsAsync(id);

            ViewData["Page"] = page ?? 0;

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View(Slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SliderModel Slider, int? page)
        {
            ViewData["Page"] = page ?? 0;

            // Kiểm tra thông tin Slider
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin Slider !!!";
                return View(Slider);
            }

            // Cập nhật và kiểm tra
            var result = await _SliderService.UpdateAsync(Slider);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(Slider);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Xóa Slider
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _SliderService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
