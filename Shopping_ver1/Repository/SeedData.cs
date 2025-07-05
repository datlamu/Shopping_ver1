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
                BrandModel russia = new BrandModel("Russia", "Russia", "Russia is best", 1);
                _context.Products.AddRange(
                    new ProductModel("Cake Matcha", "cake-matcha", "Very Good", 100000, vietnam, cake, "19f83670-25ea-4114-8b4d-94512ed5f8df.jpg"),
                    new ProductModel("Dessert Cotton", "dessert-cotton", "Good", 50000, russia, dessert, "56a503c8-4fde-4d2a-814b-22e17d3e897c.jpg")
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
