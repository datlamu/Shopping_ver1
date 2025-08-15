using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class CheckoutService : ICheckoutService
{
    private readonly DataContext _context;
    private readonly MoMoSettings _MoMosettings;
    private readonly IEmailService _emailService;
    private readonly ICouponService _couponService;
    private readonly IOrderService _orderService;
    public CheckoutService(
        DataContext context,
        IEmailService emailService,
        ICouponService couponService,
        IOrderService orderService,
        IOptions<MoMoSettings> MoMosettings)
    {
        _context = context;
        _emailService = emailService;
        _couponService = couponService;
        _orderService = orderService;
        _MoMosettings = MoMosettings.Value;
    }

    // Thanh toán khi nhận hàng
    public async Task<OperationResult> CheckoutAsync(string userEmail, CartViewModel cartItemVM)
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
            var orderDetails = cartItems.Select(ci => new OrderDetailModel(userEmail, orderCode, ci)).ToList();
            await _context.OrderDetails.AddRangeAsync(orderDetails);

            // Cập nhật tồn kho
            foreach (var item in cartItems)
            {
                inventories[item.ProductId].QuantityInStock -= item.Quantity;
            }

            // Lưu database
            await _context.SaveChangesAsync();

            // Gửi email 
            var toEmail = userEmail;
            //var toEmail = "172100119@dntu.edu.vn"; // test email
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

    // Lưu tạm đơn hàng trước khi thanh toán
    public async Task<(OperationResult result, OrderModel order)> CreatePendingOrderAsync(string userEmail, CartViewModel cartItemVM)
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
                return (new OperationResult(false, $"Sản phẩm {item.ProductName} không đủ hàng !!!"), new OrderModel());
            }
        }

        // Nếu có mã giảm giá
        if (!string.IsNullOrEmpty(cartItemVM.CouponCode))
        {
            // Tìm Coupon theo code
            var coupon = await _couponService.FindByCodeAsync(cartItemVM.CouponCode);

            // Kiểm tra tính hợp lệ
            var result = _couponService.ValidateCoupon(coupon, cartItemVM.TotalProductPrice);
            if (!result.Success)
            {
                return (new OperationResult(result.Success, result.Message), new OrderModel());
            }
        }
        // Lưu tạm đơn hàng và chi tiết đơn hàng
        var orderCode = Guid.NewGuid().ToString();
        var order = new OrderModel
        {
            OrderCode = orderCode,
            UserName = userEmail,
            TotalProductPrice = cartItemVM.TotalProductPrice,
            ShippingFee = cartItemVM.Shipping.Price,
            DiscountValue = cartItemVM.DiscountValue,
            CouponCode = cartItemVM.CouponCode,
            TotalPayment = cartItemVM.TotalPayment,
            ShippingRegion = $"{cartItemVM.Shipping.City}, {cartItemVM.Shipping.District}, {cartItemVM.Shipping.Ward}",
            Status = 3
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Lưu chi tiết đơn hàng tạm
        var orderDetail = cartItems.Select(ci => new OrderDetailModel(userEmail, orderCode, ci)).ToList();
        await _context.OrderDetails.AddRangeAsync(orderDetail);
        await _context.SaveChangesAsync();

        return (new OperationResult(true, ""), order);
    }

    // Thanh toán với MoMo
    public async Task<string> CreatePaymentMoMoAsync(int amount, string orderInfo, string orderCode)
    {
        string orderId = orderCode;
        string requestId = Guid.NewGuid().ToString();

        string rawHash =
            $"accessKey={_MoMosettings.AccessKey}" +
            $"&amount={amount}" +
            $"&extraData=" +
            $"&ipnUrl={_MoMosettings.NotifyUrl}" +
            $"&orderId={orderId}" +
            $"&orderInfo={orderInfo}" +
            $"&partnerCode={_MoMosettings.PartnerCode}" +
            $"&redirectUrl={_MoMosettings.ReturnUrl}" +
            $"&requestId={requestId}" +
            $"&requestType=captureWallet";

        string signature = CreateSignature(rawHash, _MoMosettings.SecretKey);

        var request = new
        {
            partnerCode = _MoMosettings.PartnerCode,
            accessKey = _MoMosettings.AccessKey,
            requestId,
            amount = amount.ToString(),
            orderId,
            orderInfo,
            redirectUrl = _MoMosettings.ReturnUrl,
            ipnUrl = _MoMosettings.NotifyUrl,
            extraData = "",
            requestType = _MoMosettings.RequestType,
            signature,
            lang = "vi"
        };

        using (var client = new HttpClient())
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_MoMosettings.Endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            //return ("responseString:" + responseString + ", rawHash:" + rawHash + ", request:" + request);
            return result.payUrl;
        }
    }

    // Xác nhận đặt hàng
    public async Task<bool> ConfirmOrderAsync(string orderCode)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        if (order == null) return false;

        var orderDetail = await _context.OrderDetails
            .Where(d => d.OrderCode == order.OrderCode)
            .ToListAsync();

        // Trừ tồn kho
        var productIds = orderDetail.Select(d => d.ProductId).ToList();
        var inventories = await _context.Inventories
            .Where(i => productIds.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId);

        foreach (var item in orderDetail)
        {
            if (inventories.ContainsKey(item.ProductId))
                inventories[item.ProductId].QuantityInStock -= item.Quantity;
        }

        // Giảm coupon
        if (!string.IsNullOrEmpty(order.CouponCode))
        {
            var coupon = await _couponService.FindByCodeAsync(order.CouponCode);
            if (coupon != null) coupon.Quantity -= 1;
        }

        // Đánh dấu đã thanh toán
        order.Status = 1;
        await _context.SaveChangesAsync();

        // Gửi email xác nhận
        await _emailService.SendEmailAsync(order.UserName, "Xác nhận thanh toán", $"Đơn hàng {order.OrderCode} đã được thanh toán thành công.");

        return true;
    }

    // Hủy đơn
    public async Task<bool> CancelOrderAsync(string orderCode)
    {
        var order = await _context.Orders.Where(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        if (order == null) return false;

        // Chỉ xóa nếu là đơn chưa xủ lý
        if (order.Status != 3) return false;

        var orderDetails = await _context.OrderDetails.Where(o => o.OrderCode == orderCode).ToListAsync();
        if (orderDetails.Any()) return false;

        _context.OrderDetails.RemoveRange(orderDetails);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    // Tạo khóa mã hóa cho thanh toán MoMo
    private string CreateSignature(string rawData, string secretKey)
    {
        var encoding = new UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(secretKey);
        byte[] messageBytes = encoding.GetBytes(rawData);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
            return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
        }
    }
}

