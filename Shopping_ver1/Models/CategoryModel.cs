using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thể loại")]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public int Status { get; set; }
        public CategoryModel() { }
        public CategoryModel(string name, string slug, string description, int status)
        {
            this.Name = name;
            this.Slug = slug;
            this.Description = description;
            this.Status = status;
        }
    }
}
