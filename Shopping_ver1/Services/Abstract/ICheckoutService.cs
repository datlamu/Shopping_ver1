using Shopping_ver1.Models;
using Shopping_ver1.Models.ViewModels;

namespace Shopping_ver1.Services.Abstract
{
    public interface ICheckoutService
    {
        // Thanh toán
        Task<OperationResult> CheckoutAsync(string userEmail, CartItemViewModel cartItemVM);
    }
}
