using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class CouponModel
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Yêu cầu nhập mã cho coupon này")]
    [StringLength(20)]
    public string Code { get; set; }

    [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Yêu cầu chọn kiểu giảm giá")]
    public string DiscountType { get; set; } // DiscountType: "percentage" = giảm theo %, "fixed" = giảm số tiền cố định

    [Required(ErrorMessage = "Yêu cầu điền vào số tiền giảm")]
    [Range(1, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn hoặc bằng 1")]
    [Precision(18, 0)]
    public decimal DiscountValue { get; set; } = 0;

    [Required(ErrorMessage = "Yêu cầu nhập số tiền tối thiểu cần để sử dụng mã giảm giá")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn hoặc bằng 0")]
    [Precision(18, 0)]
    public decimal MinimumOrderAmount { get; set; }

    [Required(ErrorMessage = "Yêu cầu chọn ngày bắt đầu giảm")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Yêu cầu chọn ngày kết thúc giảm")]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "Yêu cầu nhập số lượng cho coupon này")]
    public int Quantity { get; set; } = 1;

    public bool IsActive { get; set; } = true;


}
