using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Shopping_ver1.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập đánh giá sản phẩm")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }
        public int Star { get; set; }
        public ProductModel Product { get; set; }


    }
}
