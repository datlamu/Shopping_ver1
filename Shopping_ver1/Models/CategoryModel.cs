using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên danh mục")]
        public string Name { get; set; }
        public string Slug { get; set; }
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
