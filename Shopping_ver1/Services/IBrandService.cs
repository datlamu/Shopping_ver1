using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IBrandService
    {
        // Tìm kiếm thương hiệu chỉnh sửa
        Task<BrandModel?> GetUpdateBrandAsync(int id);
        // Lấy danh sách thương hiệu
        Task<(List<BrandModel> data, Paginate pager)> GetBrandlistAsync(int page);
        // Tạo thương hiệu mới
        Task<OperationResult> CreateBrandAsync(BrandModel brand);
        // Tạo thương hiệu mới
        Task<OperationResult> UpdateBrandAsync(BrandModel brand);
        // Xóa danh mục
        Task<OperationResult> DeleteBrandAsync(int id);
    }
}
