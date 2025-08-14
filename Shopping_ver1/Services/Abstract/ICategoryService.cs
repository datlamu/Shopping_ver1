using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICategoryService
    {
        // Lấy danh sách thể loại
        Task<List<CategoryModel>> GetlistItemAsync();
        // Tạo thể loại mới
        Task<OperationResult> CreateAsync(CategoryModel category);
        // Tìm kiếm thể loại chỉnh sửa
        Task<CategoryModel> FindByIdAsync(int id);
        Task<CategoryModel> FindBySlugAsync(string slug);
        // Cập nhật thể loại
        Task<OperationResult> UpdateAsync(CategoryModel category);
        // Xóa thể loại
        Task<OperationResult> DeleteAsync(int id);
    }
}
