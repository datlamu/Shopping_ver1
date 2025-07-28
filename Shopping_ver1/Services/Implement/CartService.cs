using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class CartService : ICartService
{
    private readonly DataContext _dataContext;
    private readonly IProductService _productService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(DataContext context, IHttpContextAccessor httpContextAccessor, IProductService productService)
    {
        _dataContext = context;
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
    }

    // Lấy danh sách đơn hàng
    public CartItemViewModel GetListCartItem()
    {
        // Lấy danh sách sản phẩm từ giỏ hàng từ session - Nếu session không tồn tại thì trả về giỏ hàng rỗng
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new CartItemViewModel();
        var cartItem = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

        // Tạo ViewModel danh sách sản phẩm và tổng tiền
        var cartItemViewModel = new CartItemViewModel()
        {
            CartItems = cartItem,
            GrandTotal = cartItem.Sum(x => x.Total),
        };

        return cartItemViewModel;
    }

    // Thêm sản phẩm vào giỏ hàng
    public async Task<OperationResult> AddAsync(int id)
    {
        try
        {
            // Lấy giỏ hàng từ session
            var session = _httpContextAccessor.HttpContext.Session;
            if (session == null)
                return new OperationResult(false, "Không thể truy cập session.");
            var cart = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Kiểm tra sản phẩm này có trong giỏ hàng chưa
            // Chưa có thì thêm mới - Có rồi thì tăng số lượng
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem == null)
            {
                var product = await _dataContext.Products.FindAsync(id);
                if (product == null)
                    return new OperationResult(false, "Sản phẩm không tồn tại.");
                cart.Add(new CartItemModel(product));
            }
            else
                cartItem.Quantity += 1;

            // Cập nhật giỏ hàng vào session
            session.SetJson("Cart", cart);

            return new OperationResult(true, "Thêm vào giỏ hàng thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Đã có lỗi khi thêm sản phẩm này!!!");
        }
    }

    // Tăng số lượng sản phẩm trong giỏ hàng
    public async Task<OperationResult> Increase(int id)
    {
        // Lấy session
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new OperationResult(false, "Không thể truy cập session.");

        // Kiểm tra sản phẩm này có trong giỏ hàng chưa - Có thì tăng số lượng
        var cart = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
        if (cartItem == null)
            return new OperationResult(false, "Sản phẩm không tồn tại trong giỏ hàng.");

        var product = await _productService.FindProductsAsync(id);
        if (cartItem.Quantity < product.Inventory.QuantityInStock)
            cartItem.Quantity += 1;
        else
            return new OperationResult(false, $"Hiện chỉ còn {product.Inventory.QuantityInStock} sản phẩm trong kho");

        // Cập nhật giỏ hàng vào session
        session.SetJson("Cart", cart);

        return new OperationResult(true, "Tăng số lượng sản phẩm thành công!!!");
    }

    // Giảm số lượng sản phẩm trong giỏ hàng
    public OperationResult Decrease(int id)
    {
        // Lấy giỏ hàng từ session
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new OperationResult(false, "Không thể truy cập session.");
        var cart = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

        // Kiểm tra sản phẩm này có trong giỏ hàng chưa - Có thì giảm số lượng
        var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
        if (cartItem == null)
            return new OperationResult(false, "Sản phẩm không tồn tại trong giỏ hàng.");

        // Giảm số lượng trong giỏ hàng - Nếu số lượng sản phẩm dưới 1 thì xóa khỏi giỏ hàng
        if (cartItem.Quantity > 1)
            cartItem.Quantity -= 1;
        else
            cart.Remove(cartItem);

        // Xóa nếu giỏ hàng trống hoặc Cập nhật giỏ hàng vào session
        if (cart.Count == 0)
            session.Remove("Cart");
        else
            session.SetJson("Cart", cart);

        return new OperationResult(true, "Giảm số lượng sản phẩm này thành công!!!");
    }

    // Giảm số lượng sản phẩm trong giỏ hàng
    public OperationResult UpdateQuantityCart(int id, int quantity)
    {
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new OperationResult(false, "Không thể truy cập session");

        var cart = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        var item = cart.FirstOrDefault(x => x.ProductId == id);
        if (item == null)
            return new OperationResult(false, "Sản phẩm không tồn tại trong giỏ");

        if (quantity < 1)
            return new OperationResult(false, "Số lượng phải lơn hơn 0");

        item.Quantity = quantity;
        session.SetJson("Cart", cart);

        return new OperationResult(true, "Đã cập nhật số lượng");
    }

    // Xóa sản phẩm trong giỏ hàng
    public OperationResult Remove(int id)
    {
        // Lấy giỏ hàng từ session
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new OperationResult(false, "Không thể truy cập session.");

        // Lấy giỏ hàng từ session
        var cart = session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

        // Xóa sản phẩm khỏi giỏ hàng
        cart.RemoveAll(c => c.ProductId == id);

        // Xóa hoặc Cập nhật giỏ hàng vào session
        if (cart.Count == 0)
            session.Remove("Cart");
        else
            session.SetJson("Cart", cart);

        return new OperationResult(true, "Xóa sản phẩm này thành công!!!");
    }

    // Làm mới giỏ hàng
    public OperationResult Clear()
    {
        // Lấy giỏ hàng từ session
        var session = _httpContextAccessor.HttpContext.Session;
        if (session == null)
            return new OperationResult(false, "Không thể truy cập session.");

        // Xóa giỏ hàng ở session
        session.Remove("Cart");

        return new OperationResult(true, "Đã xóa toàn bộ sản phẩm trong giỏ hàng!!!");
    }

    public async Task<ShippingModel> GetShippingAsync(string city, string district, string ward)
    {
        var esistingShopping = await _dataContext.Shippings
                   .FirstOrDefaultAsync(s => s.City == city && s.District == district && s.Ward == ward);

        decimal shippingPrice = 0;

        if (esistingShopping != null)
            shippingPrice = esistingShopping.Price;
        else
            shippingPrice = 50000;

        var shipping = new ShippingModel(shippingPrice, city, district, ward);

        return shipping;
    }
}
