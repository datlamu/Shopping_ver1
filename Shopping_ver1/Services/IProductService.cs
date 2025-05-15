using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IProductService
    {
        // Tạo slug từ tên sản phẩm
        string GenerateSlug(string name);
        // Kiểm tra slug có bị trùng trong database
        Task<bool> IsSlugUniqueAsync(string slug);
        // Lưu ảnh sản phẩm vào thư mục server và trả về tên file
        Task<string> SaveImageAsync(IFormFile imageUpload);
        // Lưu thông tin sản phẩm và ảnh vào database
        Task SaveProductAsync(ProductModel product, string imageName);
    }
}
