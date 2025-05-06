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
                CategoryModel cake = new CategoryModel("Cake", "cake", "Cake is best", 1);
                CategoryModel dessert = new CategoryModel("Dessert", "dessert", "Dessert is best", 1);
                BrandModel vietnam = new BrandModel("Vietnam", "vietnam", "Vietnam is best", 1);
                BrandModel china = new BrandModel("China", "china", "China is best", 1);
                _context.Products.AddRange(
                    new ProductModel("Cake Matcha", "cake-matcha", "Very Good", 100000, vietnam, cake, "1.jpg"),
                    new ProductModel("Dessert Cotton", "dessert-cotton", "Good", 50000, china, dessert, "2.jpg")
                );
                _context.SaveChanges();
            }
        }
    }
}
