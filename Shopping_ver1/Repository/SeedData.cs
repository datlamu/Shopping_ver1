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
                BrandModel cakeVN = new BrandModel("CakeVN", "cakeVN", "CakeVN is best", 1);
                BrandModel cakeUSA = new BrandModel("CakeUSA", "cakeUSA", "CakeUSA is best", 1);
                _context.Products.AddRange(
                    new ProductModel("Bánh kem matcha", "Bánh kem matcha", "Rất ngon", 100000, cakeVN, cake, "1.jpg"),
                    new ProductModel("Bánh kẹo bông", "Bánh kẹo bông", "Ngon", 50000, cakeUSA, dessert, "2.jpg")
                );
                _context.SaveChanges();
            }
        }
    }
}
