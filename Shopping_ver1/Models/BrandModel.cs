using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public int Status { get; set; }
        public BrandModel() { }
        public BrandModel(string name, string slug, string description, int status)
        {
            this.Name = name;
            this.Slug = slug;
            this.Description = description;
            this.Status = status;
        }
    }
}
