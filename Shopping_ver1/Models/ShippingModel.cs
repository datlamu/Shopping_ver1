using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shopping_ver1.Models
{
    public class ShippingModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập phí vận chuyển")]
        [Range(0, double.MaxValue, ErrorMessage = "Phí vận chuyển phải lớn hơn hoặc bằng 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Phí vận chuyển chỉ được chứa số")]
        [Precision(18, 0)]
        public decimal Price { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public DateTime Datecreated { get; set; } = DateTime.Now;
        public ShippingModel() { }
        public ShippingModel(decimal price, string city, string district, string ward)
        {
            Price = price;
            City = city;
            District = district;
            Ward = ward;
        }
    }
}
