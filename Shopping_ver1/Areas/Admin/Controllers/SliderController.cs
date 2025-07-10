using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services;

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
        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy danh sách và phân trang
            var (data, pager) = await _SliderService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

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
        public async Task<IActionResult> Update(int id)
        {
            // Tìm Slider đã chọn
            var Slider = await _SliderService.FindSlidersAsync(id);

            // Quay lại trang create giữ nguyên lại dữ liệu
            return View(Slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SliderModel Slider)
        {
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

            return RedirectToAction("Index");
        }

        // Xóa Slider
        public async Task<IActionResult> Delete(int id)
        {
            // Slider
            var result = await _SliderService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }

        // Tải lại table cập nhật dữ liệu mới ( ajax )
        public async Task<IActionResult> GetTable(int page = 1)
        {
            var (data, pager) = await _SliderService.GetlistItemAsync(page);

            ViewBag.Pager = pager;

            return PartialView("_SliderTablePartial", data);
        }
    }
}
