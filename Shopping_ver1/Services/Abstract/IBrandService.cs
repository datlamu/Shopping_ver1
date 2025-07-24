using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface IBrandService
    {
        // Lấy danh sách thương hiệu
        Task<List<BrandModel>> GetlistItemAsync();
        // Tạo thương hiệu mới
        Task<OperationResult> CreateAsync(BrandModel category);
        // Tìm kiếm thương hiệu chỉnh sửa
        Task<BrandModel> GetUpdateItemAsync(int id);
        // Cập nhật thương hiệu
        Task<OperationResult> UpdateAsync(BrandModel category);
        // Xóa thương hiệu
        Task<OperationResult> DeleteAsync(int id);
    }
}
