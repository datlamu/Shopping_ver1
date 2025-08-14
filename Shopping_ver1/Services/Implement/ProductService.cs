using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class ProductService : IProductService
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _env;

    public ProductService(DataContext context, IWebHostEnvironment env)
    {
        _dataContext = context;
        _env = env;
    }

    //  Tạo slug từ tên sản phẩm
    public string GenerateSlug(string name)
    {
        name = name.ToLowerInvariant(); // Chuyển tất cả ký tự thành chữ thường
        name = Regex.Replace(name, @"\s+", "-"); // Thay thế các khoảng trắng liên tiếp bằng dấu "-"
        name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // Loại bỏ các ký tự không phải chữ cái, số và dấu "-"
        name = Regex.Replace(name, @"-+", "-").Trim('-'); // Xử lý dấu "-" dư thừa ở đầu và cuối chuỗi
        return name;
    }

    // Kiểm tra slug có bị trùng trong cơ sở dữ liệu
    public async Task<bool> IsSlugUnique(string slug)
    {
        return !await _dataContext.Products.AnyAsync(c => c.Slug == slug);
    }

    // Xử lý ảnh sau đó trả về tên của ảnh đó
    public async Task<string> SaveImage(IFormFile imageUpload)
    {
        // Đảo bảo đã có file được upload
        if (imageUpload == null || imageUpload.Length == 0)
            throw new ArgumentException("Image is required");

        // Đường dẫn nới lưu trữ hình ảnh ( nếu chưa có folder đó thì tạo )
        string uploadDir = Path.Combine(_env.WebRootPath, "media/products");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        // Sửa tên ảnh để tránh bị trùng
        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageUpload.FileName);
        // Đường dẫn đầy đủ để lưu ảnh
        string filePath = Path.Combine(uploadDir, imageName);

        // Using để giúp xử lý các lỗi bất ngờ và chắc chắn luôn tự giải phóng khi dùng xong
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            // Lưu ảnh vào thư mục
            await imageUpload.CopyToAsync(fs);
        }

        return imageName;
    }

    // Lấy danh sách thể loại và thương hiệu
    public async Task<(SelectList Categories, SelectList Brands)> GetCategoryAndBrandListAsync(int? categoryId = null, int? brandId = null)
    {
        var categories = await _dataContext.Categories.ToListAsync();
        var brands = await _dataContext.Brands.ToListAsync();

        var categorySelectList = new SelectList(categories, "Id", "Name", categoryId);
        var brandSelectList = new SelectList(brands, "Id", "Name", brandId);

        return (categorySelectList, brandSelectList);
    }

    // Lấy danh sách sản phẩm
    public async Task<List<ProductModel>> GetAllAsync()
    {
        return await _dataContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .ToListAsync();
    }

    // Lọc sản phẩm
    private IQueryable<ProductModel> SortProducts(IQueryable<ProductModel> productsAQ, string sort_by)
    {
        switch (sort_by)
        {
            case "price_increase":
                productsAQ = productsAQ.OrderBy(p => p.Price);
                break;
            case "price_decrease":
                productsAQ = productsAQ.OrderByDescending(p => p.Price);
                break;
            case "newest":
                productsAQ = productsAQ.OrderByDescending(p => p.Id);
                break;
            case "oldest":
                productsAQ = productsAQ.OrderBy(p => p.Id);
                break;
            default:
                break;
        }
        return productsAQ;
    }

    // Lấy danh sách sản phẩm và phân trang ( có lọc sản phẩm )
    public async Task<(List<ProductModel> data, Paginate pager)> GetPagedProductListAsync(IQueryable<ProductModel> productsAQ, string sort_by, int page)
    {
        try
        {
            // Lọc sản phẩm
            productsAQ = SortProducts(productsAQ, sort_by);

            // Tạo phân trang
            var totalItems = await productsAQ.CountAsync();
            var pager = new Paginate(totalItems, page, 6);

            // Danh sách items
            var data = await productsAQ
                .Skip(pager.Skip)       // Bỏ qua số lượng phần tử
                .Take(pager.PageSize)   // Lấy số lượng phần tử tiếp đó
                .ToListAsync();

            return (data, pager);
        }
        catch
        {
            return (new List<ProductModel>(), new Paginate());
        }
    }

    // Tìm kiếm
    public async Task<List<ProductModel>> SearchItem(string search)
    {
        try
        {
            // Tìm kiếm các sản phẩm
            var products = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p =>
                    EF.Functions.Like(p.Name, $"%{search}%") ||
                    EF.Functions.Like(p.Description, $"%{search}%") ||
                    EF.Functions.Like(p.Brand.Name, $"%{search}%") ||
                    EF.Functions.Like(p.Brand.Description, $"%{search}%") ||
                    EF.Functions.Like(p.Category.Name, $"%{search}%") ||
                    EF.Functions.Like(p.Category.Description, $"%{search}%")
                )
                .ToListAsync();

            return products;
        }
        catch
        {
            return new List<ProductModel>();
        }
    }

    // Sản phẩm liên quan
    public async Task<List<ProductModel>> RelatedByCategoryAsync(int categoryId, int productId)
    {
        // Tìm sản phẩm liên quan theo thể loại
        var relatedProducts = await _dataContext.Products
            .Where(p => p.CategoryId == categoryId && p.Id != productId)
            .ToListAsync();

        return relatedProducts;
    }

    // Tạo sản phẩm mới
    public async Task<OperationResult> CreateAsync(ProductModel product)
    {
        try
        {
            // Lấy ra slug dựa vào tên sản phẩm
            product.Slug = GenerateSlug(product.Name);

            // Kiểm tra xem sản phẩm này tồn tại chưa
            var result = await IsSlugUnique(product.Slug);
            if (!result)
                return new OperationResult(false, "Sản phẩm này đã tồn tại !!!");

            // Chắc chắn chắn chọn ảnh và đảm bảo không phải ảnh rác hoặc lỗi 
            if (product.ImageUpload == null || product.ImageUpload.Length == 0)
                return new OperationResult(false, "Vui lòng chọn ảnh cho sản phẩm");

            // Lưu ảnh
            product.Image = await SaveImage(product.ImageUpload);

            // Thêm sản phẩm mới
            await _dataContext.Products.AddAsync(product);
            await _dataContext.SaveChangesAsync();

            // Đưa sản phẩm vào tồn kho
            var inventory = new InventoryModel() { ProductId = product.Id };
            await _dataContext.Inventories.AddAsync(inventory);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm sản phẩm mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm sản phẩm thất bại!!!");
        }
    }

    // Tìm kiếm sản phẩm
    public async Task<ProductModel> FindProductsAsync(int id)
    {
        return await _dataContext.Products.Include(p => p.Inventory).FirstOrDefaultAsync(p => p.Id == id);
    }

    // Cập nhật sản phẩm
    public async Task<OperationResult> UpdateAsync(ProductModel product)
    {
        try
        {
            // Lấy ra slug dựa vào tên sản phẩm
            product.Slug = GenerateSlug(product.Name);

            // Lấy slug cũ để kiểm tra
            var oldSlug = await _dataContext.Products
                .AsNoTracking()
                .Where(p => p.Id == product.Id)
                .Select(p => p.Slug)
                .FirstOrDefaultAsync();

            // Kiểm tra xem sản phẩm này tồn tại chưa
            var result = await IsSlugUnique(product.Slug);
            if (product.Slug != oldSlug && !result)
                return new OperationResult(false, "Sản phẩm này đã tồn tại !!!");

            // Chắc chắn chắn chọn ảnh và đảm bảo không phải ảnh rác hoặc lỗi 
            if (product.ImageUpload != null)
            {
                if (product.ImageUpload == null || product.ImageUpload.Length == 0)
                    return new OperationResult(false, "Vui lòng chọn ảnh sản phẩm");

                // Xóa ảnh trong wroot
                if (product.Image != "default.png")
                {
                    string path = Path.Combine(_env.WebRootPath, "media/products", product.Image);
                    if (File.Exists(path))
                        File.Delete(path);
                }
                // Lưu ảnh
                product.Image = await SaveImage(product.ImageUpload);
            }

            // Cập nhật sản phẩm
            _dataContext.Products.Update(product);

            // Cập nhật số lượng tồn kho
            _dataContext.Attach(product.Inventory);
            _dataContext.Entry(product.Inventory).Property(i => i.QuantityInStock).IsModified = true;

            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật sản phẩm mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật sản phẩm thất bại!!!");
        }
    }

    // Xóa sản phẩm
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm sản phẩm
            var product = await _dataContext.Products.Include(p => p.Inventory).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return new OperationResult(false, "Không tìm thấy sản phẩm");

            // Xóa ảnh trong wroot
            if (product.Image != "default.png")
            {
                string path = Path.Combine(_env.WebRootPath, "media/products", product.Image);
                if (File.Exists(path))
                    File.Delete(path);
            }

            // Xóa tồn kho và sản phẩm
            _dataContext.Inventories.Remove(product.Inventory);
            _dataContext.Products.Remove(product);

            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa sản phẩm thành công");
        }
        catch
        {
            return new OperationResult(false, "Xóa sản phẩm thất bại");
        }
    }

    // Đánh giá sản phẩm
    public async Task<OperationResult> ItemReview(RatingModel rating)
    {
        try
        {
            // Tìm sản phẩm
            await _dataContext.Ratings.AddAsync(rating);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Gửi đánh giá thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Gửi đánh giá thất bại");
        }
    }

    // Lấy thông tin đánh giá sản phẩm
    public async Task<List<RatingModel>> GetReviewProduct(int productId)
    {
        // Lấy ra tối đa 3 đánh giá - test
        return await _dataContext.Ratings.Where(r => r.ProductId == productId).Take(3).ToListAsync();
    }
}
