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
    public async Task<(List<OrderModel> data, Paginate pager)> GetOrderlistAsync(int page)
    {
        try
        {
            // Tổng số Items
            var totalItems = await _dataContext.Orders.CountAsync();
            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page);

            // Danh sách items
            var data = await _dataContext.Orders
                .OrderByDescending(p => p.Id)
                .Skip(pager.Skip)       // Bỏ qua số lượng phần tử
                .Take(pager.PageSize)   // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            return (data, pager);
        }
        catch
        {
            return (new List<OrderModel>(), new Paginate());
        }
    }

    // Chi tiết đơn hàng
    public async Task<List<OrderDetailModel>> GetOrderDetailAsync(string orderCode)
    {
        try
        {
            // Chi tiết của đơn ( dựa vào orderCode )
            var data = await _dataContext.OrderDetails
                .Include(o => o.Product)
                .Where(od => od.OrderCode == orderCode)
                .ToListAsync();

            return data;
        }
        catch
        {
            return new List<OrderDetailModel>();
        }

    }

    // Cập nhật thông tin đơn hàng
    public async Task<(bool success, string message)> UpdateOrderAsync(string orderCode, int status)
    {
        // Tìm đơn hàng và kiểm tra
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        if (order == null)
        {
            return (false, "Không tìm thấy đơn đặt hàng");
        }

        try
        {
            // Cập nhật đơn hàng
            order.Status = status;
            await _dataContext.SaveChangesAsync();

            return (true, "Cập nhật đơn hàng thành công !");
        }
        catch
        {
            return (false, "Cập nhật đơn hàng thất bại !");
        }
    }

    // Xóa đơn hàng
    public async Task<(bool success, string message)> DeleteOrderAsync(string orderCode)
    {
        // Lấy đơn hàng và kiểm tra
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        if (order == null)
        {
            return (false, "Không tìm thấy đơn hàng này !");
        }

        try
        {
            // Chi tiết đơn hàng liên quan
            var orderDetails = await _dataContext.OrderDetails
                .Where(od => od.OrderCode == orderCode)
                .ToListAsync();

            // Thực hiện xóa và lưu
            _dataContext.OrderDetails.RemoveRange(orderDetails);
            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();

            return (true, "Xóa đơn hàng thành công !");
        }
        catch
        {
            return (false, "Xóa đơn hàng thất bại !");
        }
    }
}
