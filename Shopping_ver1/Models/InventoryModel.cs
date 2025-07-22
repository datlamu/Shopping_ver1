using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class InventoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public ProductModel Product { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải >= 0")]
        [Required(ErrorMessage = "Yêu cầu nhập số lượng tồn kho")]
        public int QuantityInStock { get; set; } = 0;
    }
}
