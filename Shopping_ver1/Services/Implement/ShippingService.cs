using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class ShippingService : IShippingService
{
    private readonly DataContext _dataContext;

    public ShippingService(DataContext context)
    {
        _dataContext = context;
    }

    // Lấy danh sách phí vấn chuyển và phân trang
    public async Task<List<ShippingModel>> GetlistItemAsync()
    {
        return await _dataContext.Shippings.ToListAsync();
    }

    // Tạo phí vấn chuyển mới
    public async Task<OperationResult> CreateAsync(ShippingModel shipping)
    {
        try
        {
            var esistingShopping = await _dataContext.Shippings
                    .AnyAsync(s => s.City == shipping.City &&
                    s.District == shipping.District &&
                    s.Ward == shipping.Ward);
            if (esistingShopping)
                return new OperationResult(false, "Dữ liệu bị trùng lặp!!!");

            // Tạo mới phí vận chuyển cho khu vực này
            await _dataContext.Shippings.AddAsync(shipping);

            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm phí vấn chuyển mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm phí vấn chuyển thất bại!!!");
        }
    }

    // Tìm kiếm phí vấn chuyển chỉnh sửa
    public async Task<ShippingModel> GetUpdateItemAsync(int id)
    {
        return await _dataContext.Shippings.FindAsync(id);
    }

    // Chỉnh sửa phí vận chuyển
    public async Task<OperationResult> UpdateAsync(int id, decimal newPrice)
    {
        try
        {
            // Tìm đơn hàng và kiểm tra
            var shipping = await _dataContext.Shippings.FindAsync(id);
            if (shipping == null)
            {
                return new OperationResult(false, "Không tìm thấy thông tin phí vận chuyển!!!");
            }

            // Cập nhật đơn hàng
            shipping.Price = newPrice;
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật phí vận chuyển thành công !");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật phí vận chuyển thất bại!!!");
        }
    }
    // Xóa phí vấn chuyển
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm phí vấn chuyển
            var shipping = await _dataContext.Shippings.FindAsync(id);
            if (shipping == null)
                return new OperationResult(false, "Không tìm thấy phí vấn chuyển này!!!");

            // Xóa và lưu lại
            _dataContext.Shippings.Remove(shipping);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa phí vấn chuyển thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Xóa phí vấn chuyển thất bại!!!");
        }
    }

    // Lấy shipping
    public async Task<ShippingModel> GetShippingAsync(string city, string district, string ward)
    {
        // Lấy shipping theo khu vực
        var esistingShopping = await _dataContext.Shippings
                   .FirstOrDefaultAsync(s => s.City == city && s.District == district && s.Ward == ward);

        decimal shippingPrice = 0;

        // Nếu khu vực này chưa có thì gán giá ship là 50k
        if (esistingShopping != null)
            shippingPrice = esistingShopping.Price;
        else
            shippingPrice = 50000;

        var shipping = new ShippingModel(shippingPrice, city, district, ward);

        return shipping;
    }
}
