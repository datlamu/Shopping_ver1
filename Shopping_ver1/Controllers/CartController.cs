using Microsoft.AspNetCore.Mvc;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;

namespace Shopping_ver1.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        // Hiện sản phẩm giỏ hàng
        public IActionResult Index()
        {
            List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartItemViewModel = new()
            {
                CartItems = cartItem,
                GrandTotal = cartItem.Sum(x => x.Total),
            };

            return View(cartItemViewModel);
        }

        public IActionResult Checkout()
        {
            return View();
        }

        // Thêm vào giỏ hàng
        public async Task<IActionResult> Add(int id)
        {
            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            // Tìm sản phẩm trong giỏ hàng theo id
            CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

            // Thêm sản phẩm vào giỏ hàng
            if (cartItem == null)
            {
                ProductModel product = await _dataContext.Products.FindAsync(id);
                cart.Add(new CartItemModel(product));
            }
            else
                cartItem.Quantity += 1;

            // Cập nhật giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cart);

            // In thông báo
            TempData["success"] = "Add item to cart Successfully!!!";

            // Trả về trang trước đó ( 1 URL nên dùng cho chi tiết sản phẩm )
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Tăng số lượng sản phẩm trong giỏ hàng
        public async Task<IActionResult> Increase(int id)
        {
            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            // Tìm sản phẩm trong giỏ hàng theo id
            CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

            // Tăng số lượng của sản phẩm đó
            if (cartItem != null)
                cartItem.Quantity += 1;

            // Cập nhật giỏ hàng vào session
            if (cart.Count > 0)
                HttpContext.Session.SetJson("Cart", cart);

            // In thông báo
            TempData["success"] = "Increase item quantity to cart Successfully!!!";

            // Trả về trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Giảm số lượng sản phẩm trong giỏ hàng
        public async Task<IActionResult> Decrease(int id)
        {
            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            // Tìm sản phẩm trong giỏ hàng theo id
            CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

            // Giảm số lượng trong giỏ hàng
            if (cartItem.Quantity > 1)
                cartItem.Quantity -= 1;
            else
                cart.Remove(cartItem);

            // Xóa hoặc Cập nhật giỏ hàng vào session
            if (cart.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.SetJson("Cart", cart);

            // In thông báo
            TempData["success"] = "Decrease item quantity to cart Successfully!!!";

            // Trả về trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm trong giỏ hàng
        public async Task<IActionResult> Remove(int id)
        {
            // Lấy giỏ hàng từ session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm và xóa sản phẩm khỏi giỏ hàng
            cart.RemoveAll(c => c.ProductId == id);

            // Xóa hoặc Cập nhật giỏ hàng vào session
            if (cart.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.SetJson("Cart", cart);

            // In thông báo
            TempData["success"] = "Remove item of cart Successfully!!!";

            // Trả về trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ sản phẩm trong giỏ hàng
        public async Task<IActionResult> Clear()
        {
            // Xóa giỏ hàng ở session
            HttpContext.Session.Remove("Cart");

            // In thông báo
            TempData["success"] = "Clear all item of cart Successfully!!!";

            // Trả về trang giỏ hàng
            return RedirectToAction("Index");
        }
    }
}
