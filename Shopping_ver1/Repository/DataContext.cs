using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;

namespace Shopping_ver1.Repository
{
    public class DataContext : IdentityDbContext<UserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }


    }
}
