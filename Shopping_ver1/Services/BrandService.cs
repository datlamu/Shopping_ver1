using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
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

    // Format lại text trong Description
    private string SanitizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        var decoded = WebUtility.HtmlDecode(description);
        var noHtml = Regex.Replace(decoded, "<.*?>", string.Empty);
        return Regex.Replace(noHtml, @"\s+", " ").Trim();
    }

    // Lấy danh sách thương hiệu và phân trang
    public async Task<List<BrandModel>> GetlistItemAsync()
    {
        return await _dataContext.Brands.ToListAsync();
    }

    // Tạo thương hiệu mới
    public async Task<OperationResult> CreateAsync(BrandModel Brand)
    {
        try
        {
            // Lấy ra slug dựa vào tên thương hiệu
            Brand.Slug = GenerateSlug(Brand.Name);

            // Kiểm tra thương hiệu này tồn tại chưa
            var result = await IsSlugUnique(Brand.Slug);
            if (!result)
                return new OperationResult(false, "thương hiệu này đã tồn tại !!!");

            // Format lại text trong Description
            Brand.Description = SanitizeDescription(Brand.Description);

            // Thêm và lưu lại thương hiệu
            await _dataContext.Brands.AddAsync(Brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm thương hiệu mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm thương hiệu thất bại!!!");
        }
    }

    // Tìm kiếm thương hiệu chỉnh sửa
    public async Task<BrandModel> GetUpdateItemAsync(int id)
    {
        return await _dataContext.Brands.FindAsync(id);
    }

    // Chỉnh sửa thương hiệu
    public async Task<OperationResult> UpdateAsync(BrandModel Brand)
    {
        try
        {// Lấy ra slug dựa vào tên thương hiệu
            Brand.Slug = GenerateSlug(Brand.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Brands
                .AsNoTracking()
                .Where(c => c.Id == Brand.Id)
                .Select(c => c.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem thương hiệu này tồn tại chưa
            var result = await IsSlugUnique(Brand.Slug);
            if (Brand.Slug != oldSlug && !result)
                return new OperationResult(false, "thương hiệu này đã tồn tại !!!");

            // Format lại text trong Description
            Brand.Description = SanitizeDescription(Brand.Description);

            // Chỉnh sửa thương hiệu
            _dataContext.Brands.Update(Brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Chỉnh sửa thương hiệu thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Chỉnh sửa thương hiệu thất bại!!!");
        }
    }

    // Xóa thương hiệu
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm thương hiệu
            var Brand = await _dataContext.Brands.FindAsync(id);
            if (Brand == null)
                return new OperationResult(false, "Không tìm thấy thương hiệu này!!!");

            // Xóa và lưu lại
            _dataContext.Brands.Remove(Brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa thương hiệu thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Xóa thương hiệu thất bại!!!");
        }
    }
}
