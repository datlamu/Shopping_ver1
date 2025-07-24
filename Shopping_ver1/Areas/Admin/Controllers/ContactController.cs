using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Services.Abstract;

namespace Shopping_ver1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Ngăn không lưu cache
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // Danh sách thông tin liên hệ
        public async Task<IActionResult> Index(int? page)
        {
            // Lấy danh sách item
            var data = await _contactService.GetlistItemAsync();

            // Trang hiện tại
            ViewBag.Page = page ?? 0;

            return View(data);
        }

        // Tạo thông tin liên hệ mới
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ContactModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactModel contact)
        {
            // Kiểm tra thông tin liên hệ
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin liên hệ !!!";
                return View(contact);
            }

            // Tạo thông tin liên hệ mới và kiểm tra
            var result = await _contactService.CreateAsync(contact);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(contact);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index");
        }

        // Chỉnh sửa thông tin liên hệ
        [HttpGet]
        public async Task<IActionResult> Update(int id, int? page)
        {
            // Tìm thông tin liên hệ đã chọn
            var product = await _contactService.FindItemAsync(id);

            ViewBag.Page = page ?? 0;

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ContactModel contact, int? page)
        {
            ViewBag.Page = page ?? 0;

            // Kiểm tra thông tin liên hệ
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng kiểm tra lại thông tin liên hệ !!!";
                return View(contact);
            }

            // Cập nhật và kiểm tra
            var result = await _contactService.UpdateAsync(contact);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(contact);
            }
            TempData["Success"] = result.Message;

            return RedirectToAction("Index", new { page = page ?? 0 });
        }

        // Xóa thông tin liên hệ
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _contactService.DeleteAsync(id);

            return Json(new { result.Success, result.Message });
        }
    }
}
