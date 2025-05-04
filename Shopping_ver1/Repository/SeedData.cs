using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;

namespace Shopping_ver1.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel laptop = new CategoryModel("Macbook", "macbook", "Macbook is best", 1);
                CategoryModel pc = new CategoryModel("Pc", "pc", "Pc is best", 1);
                BrandModel apple = new BrandModel("Apple", "apple", "Apple is best", 1);
                BrandModel samsung = new BrandModel("Samsung", "samsung", "Samsung is best", 1);
                _context.Products.AddRange(
                    new ProductModel("Macbook", "macbook", "Very good", 100000, apple, laptop, "image1.jpg"),
                    new ProductModel("Pc", "Pc", "Good", 150000, samsung, pc, "image2.jpg")
                );
                _context.SaveChanges();
            }
        }
    }
}
