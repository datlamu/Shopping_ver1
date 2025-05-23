using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

public class CheckoutService : ICheckoutService
{
    private readonly DataContext _context;

    public CheckoutService(DataContext context)
    {
        _context = context;
    }

    // Thanh toán
    public async Task<string> CheckoutAsync(string userEmail, List<CartItemModel> cartItems)
    {
        // Thêm đơn hàng
        var orderCode = Guid.NewGuid().ToString();
        var orderItem = new OrderModel(orderCode, userEmail);
        _context.Add(orderItem);

        // Thêm chi tiết đơn đơn hàng
        foreach (var item in cartItems)
        {
            var orderDetails = new OrderDetailModel(orderItem, item);
            _context.Add(orderDetails);
        }

        // Lưu lại database
        await _context.SaveChangesAsync();
        return orderCode;
    }
}
