using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
{
    public interface IOrderService
    {

        // Danh sách đơn hàng
        Task<List<OrderModel>> GetOrderlistAsync();

        // Lấy danh sách đơn hàng
        Task<List<OrderModel>> FindByUserEmailAsync(string userEmail);

        // Chi tiết đơn hàng
        Task<OrderDetailViewModel> GetOrderDetailAsync(string orderCode);

        // Cập nhật thông tin đơn hàng
        Task<OperationResult> UpdateOrderAsync(string orderCode, int status);

        // Xóa đơn hàng
        Task<OperationResult> DeleteOrderAsync(int id);

    }
}
