using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

public class ProductService : IProductService
{
    private readonly DataContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductService(DataContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    //  Tạo slug từ tên sản phẩm
    public string GenerateSlug(string name)
    {
        name = name.ToLowerInvariant(); // Chuyển tất cả ký tự thành chữ thường
        name = Regex.Replace(name, @"\s+", "-"); // Thay thế các khoảng trắng liên tiếp bằng dấu "-"
        name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // Loại bỏ các ký tự không phải chữ cái, số và dấu "-"
        name = Regex.Replace(name, @"-+", "-").Trim('-'); // Xử lý dấu "-" dư thừa ở đầu và cuối chuỗi
        return name;
    }

    // Kiểm tra slug có bị trùng trong cơ sở dữ liệu
    public async Task<bool> IsSlugUniqueAsync(string slug)
    {
        return !await _context.Products.AnyAsync(p => p.Slug == slug);
    }

    // Xử lý ảnh sau đó trả về tên của ảnh đó
    public async Task<string> SaveImageAsync(IFormFile imageUpload)
    {
        // Đảo bảo đã có file được upload
        if (imageUpload == null || imageUpload.Length == 0)
            throw new ArgumentException("Image is required");

        // Đường dẫn nới lưu trữ hình ảnh ( nếu chưa có folder đó thì tạo )
        string uploadDir = Path.Combine(_env.WebRootPath, "media/products");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        // Sửa tên ảnh để tránh bị trùng
        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageUpload.FileName);
        // Đường dẫn đầy đủ để lưu ảnh
        string filePath = Path.Combine(uploadDir, imageName);

        // Using để giúp xử lý các lỗi bất ngờ và chắc chắn luôn tự giải phóng khi dùng xong
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            // Lưu ảnh vào thư mục
            await imageUpload.CopyToAsync(fs);
        }

        return imageName;
    }

    // Lưu sản phẩm vào database
    public async Task SaveProductAsync(ProductModel product, string imageName)
    {
        product.Image = imageName;
        product.Description = Regex.Replace(product.Description, "<.*?>", string.Empty);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }
}
