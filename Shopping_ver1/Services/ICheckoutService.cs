using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface ICheckoutService
    {
        // Thanh toán
        Task<string> CheckoutAsync(string userEmail, List<CartItemModel> cartItems);
    }
}
