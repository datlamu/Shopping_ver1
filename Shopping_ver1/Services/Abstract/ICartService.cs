using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICartService
    {
        // Lấy danh sách sản phẩm trong giỏ hàng
        CartItemViewModel GetListCartItem();
        // Thêm sản phẩm vào giỏ hàng
        Task<OperationResult> AddAsync(int id);
        // Tăng số lượng sản phẩm trong giỏ hàng
        Task<OperationResult> Increase(int id);
        // Giảm số lượng sản phẩm trong giỏ hàng
        OperationResult Decrease(int id);
        // Giảm số lượng sản phẩm trong giỏ hàng
        OperationResult UpdateQuantityCart(int id, int quantity);
        // Xóa sản phẩm trong giỏ hàng
        OperationResult Remove(int id);
        // Làm mới giỏ hàng
        OperationResult Clear();
        Task<ShippingModel> GetShippingAsync(string city, string district, string ward);
    }
}
