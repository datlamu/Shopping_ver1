using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class CategoryService : ICategoryService
{
    private readonly DataContext _dataContext;

    public CategoryService(DataContext context)
    {
        _dataContext = context;
    }

    //  Tạo slug từ tên thể loại
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

    // Format lại text trong Description
    private string SanitizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        var decoded = WebUtility.HtmlDecode(description);
        var noHtml = Regex.Replace(decoded, "<.*?>", string.Empty);
        return Regex.Replace(noHtml, @"\s+", " ").Trim();
    }

    // Lấy danh sách thể loại và phân trang
    public async Task<List<CategoryModel>> GetlistItemAsync()
    {
        return await _dataContext.Categories.ToListAsync();
    }

    // Tạo thể loại mới
    public async Task<OperationResult> CreateAsync(CategoryModel category)
    {
        try
        {
            // Lấy ra slug dựa vào tên thể loại
            category.Slug = GenerateSlug(category.Name);

            // Kiểm tra thể loại này tồn tại chưa
            var result = await IsSlugUnique(category.Slug);
            if (!result)
                return new OperationResult(false, "thể loại này đã tồn tại !!!");

            // Format lại text trong Description
            category.Description = SanitizeDescription(category.Description);

            // Thêm và lưu lại thể loại
            await _dataContext.Categories.AddAsync(category);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm thể loại mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm thể loại thất bại!!!");
        }
    }

    // Tìm kiếm thể loại chỉnh sửa
    public async Task<CategoryModel> GetUpdateItemAsync(int id)
    {
        return await _dataContext.Categories.FindAsync(id);
    }

    // Chỉnh sửa thể loại
    public async Task<OperationResult> UpdateAsync(CategoryModel category)
    {
        try
        {// Lấy ra slug dựa vào tên thể loại
            category.Slug = GenerateSlug(category.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Categories
                .AsNoTracking()
                .Where(c => c.Id == category.Id)
                .Select(c => c.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem thể loại này tồn tại chưa
            var result = await IsSlugUnique(category.Slug);
            if (category.Slug != oldSlug && !result)
                return new OperationResult(false, "thể loại này đã tồn tại !!!");

            // Format lại text trong Description
            category.Description = SanitizeDescription(category.Description);

            // Chỉnh sửa thể loại
            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Chỉnh sửa thể loại thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Chỉnh sửa thể loại thất bại!!!");
        }
    }

    // Xóa thể loại
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm thể loại
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
                return new OperationResult(false, "Không tìm thấy thể loại này!!!");

            // Xóa và lưu lại
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa thể loại thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Xóa thể loại thất bại!!!");
        }
    }
}
