using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

public class CategoryService : ICategoryService
{
    private readonly DataContext _dataContext;

    public CategoryService(DataContext context)
    {
        _dataContext = context;
    }

    //  Tạo slug từ tên danh mục
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
        return !await _dataContext.Categories.AnyAsync(c => c.Slug == slug);
    }

    // Lưu danh mục vào database
    public async Task SaveCategory(CategoryModel category, string action)
    {
        // Format lại text trong Description
        // Bước 1: Decode các thực thể HTML như &nbsp;, &amp;, &lt;, v.v.
        category.Description = WebUtility.HtmlDecode(category.Description);
        // Bước 2: Loại bỏ toàn bộ thẻ HTML nếu còn sót
        category.Description = Regex.Replace(category.Description, "<.*?>", string.Empty);
        // Bước 3 (tuỳ chọn): Loại bỏ khoảng trắng đầu/cuối và chuẩn hoá khoảng trắng
        category.Description = Regex.Replace(category.Description, @"\s+", " ").Trim();

        if (action == "Create")
            await _dataContext.Categories.AddAsync(category);
        else if (action == "Edit")
            _dataContext.Categories.Update(category);

        await _dataContext.SaveChangesAsync();
    }
    public async Task<bool> DeleteCategory(int id)
    {
        try
        {
            // Tìm danh mục
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null) return false;

            // Xóa và lưu lại
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
