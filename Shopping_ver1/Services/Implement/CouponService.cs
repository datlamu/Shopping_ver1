using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class CouponService : ICouponService
{
    private readonly DataContext _dataContext;

    public CouponService(DataContext context)
    {
        _dataContext = context;
    }

    // Lấy danh sách coupon
    public async Task<List<CouponModel>> GetAllAsync()
    {
        return await _dataContext.Coupons.ToListAsync();
    }

    // Tạo coupon mới
    public async Task<OperationResult> CreateAsync(CouponModel coupon)
    {
        try
        {
            // Kiểm tra trùng mã
            var couponUnique = await _dataContext.Coupons.AnyAsync(c => c.Code == coupon.Code);
            if (couponUnique)
                return new OperationResult(false, "Mã giảm giá này đã tồn tại");

            // Nếu giảm theo %
            if (coupon.DiscountType == "percentage")
            {
                if (coupon.DiscountValue > 100)
                    return new OperationResult(false, "Hãy nhập lại giảm giá vì không thể giảm hơn 100%");
            }
            // Thêm và lưu lại coupon
            await _dataContext.Coupons.AddAsync(coupon);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm mã giảm giá mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm mã giảm giá thất bại!!!");
        }
    }

    // Tìm kiếm coupon chỉnh sửa
    public async Task<CouponModel> FindByIdAsync(int id)
    {
        return await _dataContext.Coupons.FindAsync(id);
    }

    // Tìm kiếm coupon chỉnh sửa
    public async Task<CouponModel> FindByCodeAsync(string code)
    {
        return await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }

    // Chỉnh sửa coupon
    public async Task<OperationResult> UpdateAsync(CouponModel coupon)
    {
        try
        {
            // Kiểm tra trùng mã
            var couponUnique = await _dataContext.Coupons.AnyAsync(c => c.Code == coupon.Code && c.Id != coupon.Id);
            if (couponUnique)
                return new OperationResult(false, "Mã giảm giá này đã tồn tại");

            // Nếu giảm theo %
            if (coupon.DiscountType == "percentage")
            {
                if (coupon.DiscountValue > 100)
                    return new OperationResult(false, "Không thể giảm hơn 100%");
            }

            // Chỉnh sửa coupon
            _dataContext.Coupons.Update(coupon);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật mã giảm giá thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật mã giảm giá thất bại!!!");
        }
    }

    // Xóa coupon
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm coupon
            var coupon = await _dataContext.Coupons.FindAsync(id);
            if (coupon == null)
                return new OperationResult(false, "Không tìm thấy mã giảm giá này!!!");

            // Xóa và lưu lại
            _dataContext.Coupons.Remove(coupon);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa mã giảm giá thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Xóa mã giảm giá thất bại!!!");
        }
    }

    public OperationResult ValidateCoupon(CouponModel coupon, decimal totalPrice)
    {
        // Kiểm tra tính hợp lệ
        if (coupon == null || !coupon.IsActive || coupon.EndDate < DateTime.Now)
        {
            return new OperationResult(false, "Mã giảm giá không hợp lệ hoặc đã hết hạn !!!");
        }

        // Kiểm tra số lượng còn lại
        if (coupon.Quantity <= 0)
        {
            return new OperationResult(false, "Mã giảm giá này đã hết lượt sử dụng !!!");
        }

        // Kiểm tra điều kiện kích hoạt
        if (totalPrice < coupon.MinimumOrderAmount)
        {
            return new OperationResult(false, $"Không thành công!!! Đơn tối thiểu để sử dụng mã là {coupon.MinimumOrderAmount}");
        }

        return new OperationResult(true, "Áp dụng mã giảm giá thành công !!!");
    }

}
