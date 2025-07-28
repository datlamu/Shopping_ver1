using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;


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
    public async Task<OperationResult> CheckoutAsync(string userEmail, List<CartItemModel> cartItems, ShippingModel shipping)
    {
        try
        {
            // Lấy tồn kho từ giỏ hàng
            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var inventories = await _context.Inventories
                .Where(i => productIds.Contains(i.ProductId))
                .ToDictionaryAsync(i => i.ProductId);

            // Kiểm tra tồn kho
            foreach (var item in cartItems)
            {
                if (!inventories.TryGetValue(item.ProductId, out var inventory) || inventory.QuantityInStock < item.Quantity)
                {
                    return new OperationResult(false, $"Sản phẩm {item.ProductName} không đủ hàng !!!");
                }
            }

            // Tính tổng tiền sản phẩm
            decimal totalProductPrice = cartItems.Sum(ci => ci.Total);
            // Lấy phí ship
            decimal shippingFee = shipping.Price;
            // Tổng số tiền thanh toán
            decimal totalPayment = totalProductPrice + shippingFee;
            // Khu vực ship
            string shippingRegion = $"{shipping.City}, {shipping.District}, {shipping.Ward}";

            // Thêm đơn hàng
            var orderCode = Guid.NewGuid().ToString();
            var order = new OrderModel()
            {
                OrderCode = orderCode,
                UserName = userEmail,
                TotalProductPrice = totalProductPrice,
                ShippingFee = shippingFee,
                TotalPayment = totalPayment,
                ShippingRegion = shippingRegion
            };
            await _context.Orders.AddAsync(order);

            // Thêm chi tiết đơn đơn hàng
            var orderDetails = cartItems.Select(ci => new OrderDetailModel(order, ci)).ToList();
            await _context.OrderDetails.AddRangeAsync(orderDetails);

            // Cập nhật tồn kho
            foreach (var item in cartItems)
            {
                inventories[item.ProductId].QuantityInStock -= item.Quantity;
            }

            // Lưu database
            await _context.SaveChangesAsync();

            // Gửi email 
            //var toEmail = userEmail;
            var toEmail = "172100119@dntu.edu.vn"; // test email
            var subject = "Đặt hàng thành công !";
            var body = $"Cảm ơn bạn đã ủng hộ, Mã đơn hàng của bạn là: {orderCode}!";
            _ = Task.Run(async () =>
            {
                await _emailService.SendEmailAsync(toEmail, subject, body);
            });

            // Đặt hàng thành công
            return new OperationResult(true, $"Đặt hàng thành công! Mã đơn hàng của bạn là: {orderCode}");
        }
        catch
        {
            return new OperationResult(false, $"Có lỗi khi đặt hàng !!!");
        }
    }
}
