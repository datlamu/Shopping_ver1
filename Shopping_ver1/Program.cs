using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;
using Shopping_ver1.Services.Implement;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json, appsettings.Development file, and environment variables
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

// 1. DBContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));

// 2. Add Controllers + Views
builder.Services.AddControllersWithViews();

// 3. Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true;
});

// 4. Add Dependency Injection
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISliderService, SliderService>();
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<IShippingService, ShippingService>();
builder.Services.AddTransient<ICouponService, CouponService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.Configure<MoMoSettings>(builder.Configuration.GetSection("MoMoSettings"));

// 5. Identity
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
});

// 6. Google Login
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
});

var app = builder.Build();

// 11. Cookie Policy
var cookiePolicy = new CookiePolicyOptions
{
    Secure = app.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always
};

// 7. Error Page
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

// 12. Ngrok setting
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 12. Change all http to https
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

// 8. Session + Auth
app.UseSession();
app.UseCookiePolicy(cookiePolicy);
app.UseAuthentication();
app.UseAuthorization();

// 9. Routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 10. Seeding Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedData.SeedingDataAsync(context);
    await RoleInitializer.SeedRolesAsync(roleManager);
}

app.Run();
