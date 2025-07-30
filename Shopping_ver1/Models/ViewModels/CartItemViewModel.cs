namespace Shopping_ver1.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal TotalProductPrice { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalPayment { get; set; }
        public string DiscountValue { get; set; }
        public string CouponCode { get; set; }
        public ShippingModel Shipping { get; set; }
    }
}
