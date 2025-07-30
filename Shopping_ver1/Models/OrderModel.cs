using System.ComponentModel.DataAnnotations;

public class OrderModel
{
    [Key]
    public int Id { get; set; }

    public string OrderCode { get; set; }

    public string UserName { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public int Status { get; set; } = 0;

    public decimal TotalProductPrice { get; set; } = 0;

    public decimal ShippingFee { get; set; } = 0;

    public string DiscountValue { get; set; }

    public string CouponCode { get; set; }

    public decimal TotalPayment { get; set; } = 0;

    public string ShippingRegion { get; set; }
}
