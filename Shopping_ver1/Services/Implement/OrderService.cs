using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class OrderService : IOrderService
{
    private readonly DataContext _dataContext;

    public OrderService(DataContext context)
    {
        _dataContext = context;
    }

    // Lấy danh sách đơn hàng
    public async Task<List<OrderModel>> GetOrderlistAsync()
    {
        return await _dataContext.Orders.ToListAsync();
    }

    public async Task<OrderModel> FindByOrderCodelAsync(string orderCode)
    {
        return await _dataContext.Orders.Where(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
    }

    // Lấy danh sách đơn hàng theo UserEmail
    public async Task<List<OrderModel>> GetListByUserEmailAsync(string userEmail)
    {
        return await _dataContext.Orders.Where(o => o.UserName == userEmail).ToListAsync();
    }

    // Chi tiết đơn hàng
    public async Task<OrderDetailViewModel> GetOrderDetailAsync(string orderCode)
    {
        // Lấy ra chi tiết đơn hàng
        var orderDetails = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == orderCode).ToListAsync();

        // Lấy thông tin đơn hàng
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);

        // Đưa thông tin vào ViewModel
        return new OrderDetailViewModel()
        {
            OrderDetail = orderDetails,
            OrderCode = order.OrderCode,
            TotalProductPrice = order.TotalProductPrice,
            ShippingFee = order.ShippingFee,
            GrandTotal = order.TotalProductPrice + order.ShippingFee,
            DiscountValue = order.DiscountValue,
            CouponCode = order.CouponCode,
            TotalPayment = order.TotalPayment,
            ShippingRegion = order.ShippingRegion
        };
    }

    // Cập nhật thông tin đơn hàng
    public async Task<OperationResult> UpdateOrderAsync(string orderCode, int status)
    {
        try
        {
            // Tìm đơn hàng và kiểm tra
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return new OperationResult(false, "Không tìm thấy đơn đặt hàng");
            }

            // Cập nhật đơn hàng
            order.Status = status;
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật đơn hàng thành công !");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật đơn hàng thất bại !");
        }
    }

    // Xóa đơn hàng
    public async Task<OperationResult> DeleteOrderAsync(int id)
    {
        try
        {
            // Lấy đơn hàng và kiểm tra
            var order = await _dataContext.Orders.FindAsync(id);
            if (order == null)
            {
                return new OperationResult(false, "Không tìm thấy đơn hàng này !");
            }

            // Chi tiết đơn hàng liên quan
            var orderDetails = await _dataContext.OrderDetails
                .Where(od => od.OrderCode == order.OrderCode)
                .ToListAsync();

            // Thực hiện xóa và lưu
            _dataContext.OrderDetails.RemoveRange(orderDetails);
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Hủy đơn hàng thành công !");
        }
        catch
        {
            return new OperationResult(false, "Hủy đơn hàng thất bại !");
        }
    }
}
