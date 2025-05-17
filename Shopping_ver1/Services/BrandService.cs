using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

public class BrandService : IBrandService
{
    private readonly DataContext _dataContext;

    public BrandService(DataContext context)
    {
        _dataContext = context;
    }

    //  Tạo slug từ tên thương hiệu
    public string GenerateSlug(string name)
    {
        name = name.ToLowerInvariant(); // Chuyển tất cả ký tự thành chữ thường
        name = Regex.Replace(name, @"\s+", "-"); // Thay thế các khoảng trắng liên tiếp bằng dấu "-"
        name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // Loại bỏ các ký tự không phải chữ cái, số và dấu "-"
        name = Regex.Replace(name, @"-+", "-").Trim('-'); // Xử lý dấu "-" dư thừa ở đầu và cuối chuỗi
        return name;
    }

    // Kiểm tra slug có bị trùng trong cơ sở dữ liệu
    public async Task<bool> IsSlugUnique(string slug)
    {
        return !await _dataContext.Brands.AnyAsync(c => c.Slug == slug);
    }

    // Lưu thương hiệu vào database
    public async Task SaveBrand(BrandModel brand, string action)
    {
        // Format lại text trong Description
        // Bước 1: Decode các thực thể HTML như &nbsp;, &amp;, &lt;, v.v.
        brand.Description = WebUtility.HtmlDecode(brand.Description);
        // Bước 2: Loại bỏ toàn bộ thẻ HTML nếu còn sót
        brand.Description = Regex.Replace(brand.Description, "<.*?>", string.Empty);
        // Bước 3 (tuỳ chọn): Loại bỏ khoảng trắng đầu/cuối và chuẩn hoá khoảng trắng
        brand.Description = Regex.Replace(brand.Description, @"\s+", " ").Trim();

        if (action == "Create")
            await _dataContext.Brands.AddAsync(brand);
        else if (action == "Edit")
            _dataContext.Brands.Update(brand);

        await _dataContext.SaveChangesAsync();
    }
    public async Task<bool> DeleteBrand(int id)
    {
        try
        {
            // Tìm thương hiệu
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null) return false;

            // Xóa và lưu lại
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
