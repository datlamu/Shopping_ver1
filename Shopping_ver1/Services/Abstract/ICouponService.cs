using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICouponService
    {
        // Lấy danh sách coupon
        Task<List<CouponModel>> GetAllAsync();
        // Tạo coupon mới
        Task<OperationResult> CreateAsync(CouponModel coupon);
        // Tìm coupon cần chỉnh sửa
        Task<CouponModel> FindByIdAsync(int id);
        // Tìm coupon cần chỉnh sửa
        Task<CouponModel> FindByCodeAsync(string code);
        // Cập nhật coupon
        Task<OperationResult> UpdateAsync(CouponModel coupon);
        // Xóa coupon
        Task<OperationResult> DeleteAsync(int id);
        // Kiểm tra coupon
        OperationResult ValidateCoupon(CouponModel coupon, decimal totalPrice);
    }
}
