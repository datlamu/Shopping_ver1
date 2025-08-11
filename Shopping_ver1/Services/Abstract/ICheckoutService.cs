using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICheckoutService
    {
        // Thanh toán
        Task<OperationResult> CheckoutAsync(string userEmail, CartViewModel cartVM);
        // Lưu tạm đơn hàng
        Task<(OperationResult result, OrderModel order)> CreatePendingOrderAsync(string userEmail, CartViewModel cartItemVM);
        // Thanh toán với MoMo
        Task<string> CreatePaymentMoMoAsync(int amount, string orderInfo, string orderCode);
        // Xác nhận đặt hàng
        Task<bool> ConfirmOrderAsync(string orderCode);
        // Hủy đơn hàng
        Task<bool> CancelOrderAsync(string orderCode);
    }
}
