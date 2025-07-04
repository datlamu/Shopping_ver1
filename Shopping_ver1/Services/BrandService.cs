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
        var decoded = WebUtility.HtmlDecode(description);
        var noHtml = Regex.Replace(decoded, "<.*?>", string.Empty);
        return Regex.Replace(noHtml, @"\s+", " ").Trim();
    }

    // Lấy danh sách thương hiệu và 
    public async Task<(List<BrandModel> data, Paginate pager)> GetBrandlistAsync(int page)
    {
        try
        {
            // Tổng số Items
            var totalItems = await _dataContext.Brands.CountAsync();
            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page);

            // Danh sách items
            var data = await _dataContext.Brands
                .OrderByDescending(p => p.Id)
                .Skip(pager.Skip)       // Bỏ qua số lượng phần tử
                .Take(pager.PageSize)   // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            return (data, pager);
        }
        catch
        {
            return (new List<BrandModel>(), new Paginate());
        }
    }

    // Tìm kiếm thương hiệu chỉnh sửa
    public async Task<BrandModel?> GetUpdateBrandAsync(int id)
    {
        return await _dataContext.Brands.FindAsync(id);
    }

    // Tạo thương hiệu mới
    public async Task<OperationResult> CreateBrandAsync(BrandModel brand)
    {
        try
        {
            // Lấy ra slug dựa vào tên thương hiệu
            brand.Slug = GenerateSlug(brand.Name);

            // Kiểm tra thương hiệu này tồn tại chưa
            var result = await IsSlugUnique(brand.Slug);
            if (!result)
                return new OperationResult(false, "Thương hiệu này đã tồn tại !!!");

            // Format lại text trong Description
            brand.Description = SanitizeDescription(brand.Description);

            // Thêm và lưu lại thương hiệu
            await _dataContext.Brands.AddAsync(brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm thương hiệu mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm thương hiệu thất bại!!!");
        }
    }

    // Chỉnh sửa thương hiệu
    public async Task<OperationResult> UpdateBrandAsync(BrandModel brand)
    {
        try
        {// Lấy ra slug dựa vào tên thương hiệu
            brand.Slug = GenerateSlug(brand.Name);

            // Lấy slug cũ nhưng không làm EF tracking đối tượng tránh trường hợp update bị xung đột
            var oldSlug = await _dataContext.Brands
                .AsNoTracking()
                .Where(c => c.Id == brand.Id)
                .Select(c => c.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem thương hiệu này tồn tại chưa
            var result = await IsSlugUnique(brand.Slug);
            if (brand.Slug != oldSlug && !result)
                return new OperationResult(false, "Thương hiệu này đã tồn tại !!!");

            // Format lại text trong Description
            brand.Description = SanitizeDescription(brand.Description);

            // Chỉnh sửa thương hiệu
            _dataContext.Brands.Update(brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Chỉnh sửa thương hiệu thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Chỉnh sửa thương hiệu thất bại!!!");
        }
    }

    // Xóa thương hiệu
    public async Task<OperationResult> DeleteBrandAsync(int id)
    {
        try
        {
            // Tìm thương hiệu
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
                return new OperationResult(false, "Không tìm thấy thương hiệu này!!!");

            // Xóa và lưu lại
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa thương hiệu thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Xóa thương hiệu thất bại!!!");
        }
    }
}
