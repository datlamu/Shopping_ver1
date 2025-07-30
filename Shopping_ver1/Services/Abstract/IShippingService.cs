using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface IShippingService
    {
        // Lấy danh sách thể loại
        Task<List<ShippingModel>> GetlistItemAsync();
        // Tạo thể loại mới
        Task<OperationResult> CreateAsync(ShippingModel shipping);
        // Tìm kiếm thể loại chỉnh sửa
        Task<ShippingModel> GetUpdateItemAsync(int id);
        // Cập nhật thể loại
        Task<OperationResult> UpdateAsync(int id, decimal newPrice);
        // Xóa thể loại
        Task<OperationResult> DeleteAsync(int id);
        // Lấy shipping
        Task<ShippingModel> GetShippingAsync(string city, string district, string ward);
    }
}
