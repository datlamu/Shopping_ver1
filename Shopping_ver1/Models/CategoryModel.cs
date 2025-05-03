using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thể loại")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        [Required]
        public string Slug { get; set; }
        public string Status { get; set; }
    }
}
