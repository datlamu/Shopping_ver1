using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // Danh sách các coupon
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách và phân trang
            var data = await _couponService.GetAllAsync();

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Tạo coupon mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {
            // Kiểm tra thông tin coupon
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin coupon !!!";
                return View(coupon);
            }

            // Thêm coupon và kiểm tra
            var result = await _couponService.CreateAsync(coupon);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(coupon);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Cập nhật coupon
        [HttpGet]
        public async Task<IActionResult> Update(int id, int? page)
        {
            // Tìm coupon đã chọn
            var coupon = await _couponService.FindByIdAsync(id);
            if (coupon == null)
                return Json(new { Success = false, Message = "Không tìm thấy coupon này để cập nhật!!!" });

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(coupon);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CouponModel coupon, int? page)
        {
            ViewBag.Page = page ?? 0;

            // Kiểm tra thông tin coupon
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin coupon !!!";
                return View(coupon);
            }

            // Chỉnh sửa coupon và kiểm tra
            var result = await _couponService.UpdateAsync(coupon);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(coupon);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Xóa coupon
        public async Task<IActionResult> Delete(int id)
        {
            // Xóa coupon
            var result = await _couponService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
