using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Repository;
using Shopping_ver1.Services;

var builder = WebApplication.CreateBuilder(args);

// * DBConnect
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// * Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromSeconds(30);
    option.Cookie.IsEssential = true;
});

// * Add ProductService
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// * use Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
//pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");
pattern: "{area=admin}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "defaul",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// * Seeding Data 
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();
