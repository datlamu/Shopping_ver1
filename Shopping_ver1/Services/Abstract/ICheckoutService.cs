using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICheckoutService
    {
        // Thanh toán
        Task<OperationResult> CheckoutAsync(string userEmail, List<CartItemModel> cartItems, ShippingModel shipping);
    }
}
