using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface IProductService
    {
        // Lấy danh sách sản phẩm
        Task<List<ProductModel>> GetlistItemAsync();

        // Tìm kiếm
        Task<List<ProductModel>> SearchItem(string search);

        // Lấy danh sách thể loại và thương hiệu
        Task<(SelectList Categories, SelectList Brands)> GetCategoryAndBrandListAsync(int? categoryId = null, int? brandId = null);

        // Tạo sản phẩm mới
        Task<OperationResult> CreateAsync(ProductModel product);

        // Tìm kiếm sản phẩm
        Task<ProductModel> FindProductsAsync(int id);

        // Cập nhật sản phẩm
        Task<OperationResult> UpdateAsync(ProductModel product);

        // Xóa sản phẩm
        Task<OperationResult> DeleteAsync(int id);

        // Sản phẩm liên quan
        Task<List<ProductModel>> relatedItemsAsync(int categoryId, int productId);

        // Đánh giá sản phẩm
        Task<OperationResult> ItemReview(RatingModel rating);

    }
}
