using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shopping_ver1.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Precision(18, 0)]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá chỉ được chứa số")]
        public decimal Price { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn 1 thương hiệu")]
        public int BrandId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn 1 thể loại")]
        public int CategoryId { get; set; }

        public BrandModel Brand { get; set; }

        public CategoryModel Category { get; set; }

        public InventoryModel Inventory { get; set; }

        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }

        public ProductModel() { }
        public ProductModel(string name, string slug, string description, decimal price, BrandModel brand, CategoryModel category, string image)
        {
            this.Name = name;
            this.Slug = slug;
            this.Description = description;
            this.Price = price;
            this.Brand = brand;
            this.Category = category;
            this.Image = image;
        }
    }
}
