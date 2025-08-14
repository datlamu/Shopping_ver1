using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface IBrandService
    {
        // Lấy danh sách thương hiệu
        Task<List<BrandModel>> GetlistItemAsync();
        // Tạo thương hiệu mới
        Task<OperationResult> CreateAsync(BrandModel category);
        // Tìm kiếm thương hiệu để chỉnh sửa
        Task<BrandModel> FindByIdAsync(int id);
        // Lấy thương hiệu bằng slug
        Task<BrandModel> FindBySlugAsync(string slug);

        // Cập nhật thương hiệu
        Task<OperationResult> UpdateAsync(BrandModel category);
        // Xóa thương hiệu
        Task<OperationResult> DeleteAsync(int id);
    }
}
