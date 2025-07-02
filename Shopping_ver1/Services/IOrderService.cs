using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IOrderService
    {

        // Danh sách đơn hàng
        Task<(List<OrderModel> data, Paginate pager)> GetOrderlistAsync(int page);

        // Chi tiết đơn hàng
        Task<List<OrderDetailModel>> GetOrderDetailAsync(string orderCode);

        // Cập nhật thông tin đơn hàng
        Task<OperationResult> UpdateOrderAsync(string orderCode, int status);

        // Xóa đơn hàng
        Task<OperationResult> DeleteOrderAsync(string orderCode);

    }
}
