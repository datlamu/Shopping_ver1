using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;
using static Azure.Core.HttpHeader;


public class CheckoutService : ICheckoutService
{
    private readonly DataContext _context;
    private readonly IEmailService _emailService;
    private readonly ICouponService _couponService;

    public CheckoutService(DataContext context, IEmailService emailService, ICouponService couponService)
    {
        _context = context;
        _emailService = emailService;
        _couponService = couponService;
    }

    // Thanh toán
    public async Task<OperationResult> CheckoutAsync(string userEmail, CartItemViewModel cartItemVM)
    {
        try
        {
            var cartItems = cartItemVM.CartItems;
            var shipping = cartItemVM.Shipping;

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

            // Nếu có mã giảm giá
            if (!string.IsNullOrEmpty(cartItemVM.CouponCode))
            {
                // Tìm Coupon theo code
                var coupon = await _couponService.FindByCodeAsync(cartItemVM.CouponCode);

                // Kiểm tra tính hợp lệ
                var result = _couponService.ValidateCoupon(coupon, cartItemVM.TotalProductPrice);
                if (result.Success)
                {
                    coupon.Quantity -= 1; // Giảm số lượng nếu áp dụng thành công
                }
                else
                {
                    return new OperationResult(result.Success, result.Message);
                }

            }

            // Thêm đơn hàng
            var orderCode = Guid.NewGuid().ToString();
            var order = new OrderModel()
            {
                OrderCode = orderCode,
                UserName = userEmail,
                TotalProductPrice = cartItemVM.TotalProductPrice,
                ShippingFee = shipping.Price,
                DiscountValue = cartItemVM.DiscountValue,
                CouponCode = cartItemVM.CouponCode,
                TotalPayment = cartItemVM.TotalPayment,
                ShippingRegion = $"{shipping.City}, {shipping.District}, {shipping.Ward}"
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
