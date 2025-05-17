using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IBrandService
    {
        // Tạo slug từ tên thương hiệu
        string GenerateSlug(string name);
        // Kiểm tra slug có bị trùng trong database
        Task<bool> IsSlugUnique(string slug);
        // Lưu thông tin danh mục vào database
        Task SaveBrand(BrandModel brand, string action);
        // Xóa danh mục
        Task<bool> DeleteBrand(int id);
    }
}
