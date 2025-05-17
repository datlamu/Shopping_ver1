using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface ICategoryService
    {
        // Tạo slug từ tên danh mục
        string GenerateSlug(string name);
        // Kiểm tra slug có bị trùng trong database
        Task<bool> IsSlugUnique(string slug);
        // Lưu thông tin danh mục vào database
        Task SaveCategory(CategoryModel category, string action);
        // Xóa danh mục
        Task<bool> DeleteCategory(int id);
    }
}
