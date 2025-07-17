using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

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

    // Chi tiết đơn hàng
    public async Task<List<OrderDetailModel>> GetOrderDetailAsync(string orderCode)
    {
        // Chi tiết của đơn ( dựa vào orderCode )
        return await _dataContext.OrderDetails.Include(o => o.Product).Where(od => od.OrderCode == orderCode).ToListAsync();
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

            return new OperationResult(true, "Xóa đơn hàng thành công !");
        }
        catch
        {
            return new OperationResult(false, "Xóa đơn hàng thất bại !");
        }
    }
}
