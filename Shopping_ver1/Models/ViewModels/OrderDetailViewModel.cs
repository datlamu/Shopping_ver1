namespace Shopping_ver1.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public List<OrderDetailModel> OrderDetail { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalProductPrice { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }
        public string DiscountValue { get; set; }
        public string CouponCode { get; set; }
        public decimal TotalPayment { get; set; }
        public string ShippingRegion { get; set; }
    }
}
