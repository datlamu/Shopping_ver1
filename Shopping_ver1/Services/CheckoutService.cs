using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

public class CheckoutService : ICheckoutService
{
    private readonly DataContext _context;
    private readonly IEmailService _emailService;

    public CheckoutService(DataContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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

        // Đặt hàng thành công ( test email )
        var toEmail = "172100119@dntu.edu.vn";
        var subject = "Đặt hàng thành công !";
        var body = $"Cảm ơn bạn đã ủng hộ, Mã đơn hàng của bạn là: {orderCode}!";
        await _emailService.SendEmailAsync(toEmail, subject, body);

        return orderCode;
    }
}
