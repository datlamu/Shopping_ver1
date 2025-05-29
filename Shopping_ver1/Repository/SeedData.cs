using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;

namespace Shopping_ver1.Repository
{
    public class SeedData
    {
        public static async Task SeedingDataAsync(DataContext _context)
        {
            await _context.Database.MigrateAsync();
            if (!await _context.Products.AnyAsync())
            {
                CategoryModel cake = new CategoryModel("Cake", "cake", "Cake is best", 1);
                CategoryModel dessert = new CategoryModel("Dessert", "dessert", "Dessert is best", 1);
                BrandModel vietnam = new BrandModel("Vietnam", "vietnam", "Vietnam is best", 1);
                BrandModel china = new BrandModel("China", "china", "China is best", 1);
                _context.Products.AddRange(
                    new ProductModel("Cake Matcha", "cake-matcha", "Very Good", 100000, vietnam, cake, "1.jpg"),
                    new ProductModel("Dessert Cotton", "dessert-cotton", "Good", 50000, china, dessert, "2.jpg")
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
