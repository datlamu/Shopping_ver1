using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IBrandService
    {
        // Lấy danh sách thương hiệu
        Task<(List<BrandModel> data, Paginate pager)> GetlistItemAsync(int page);
        // Tạo thương hiệu mới
        Task<OperationResult> CreateAsync(BrandModel brand);
        // Tìm kiếm thương hiệu chỉnh sửa
        Task<BrandModel?> GetUpdateItemAsync(int id);
        // Tạo thương hiệu mới
        Task<OperationResult> UpdateAsync(BrandModel brand);
        // Xóa danh mục
        Task<OperationResult> DeleteAsync(int id);
    }
}
