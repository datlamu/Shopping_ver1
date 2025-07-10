using Microsoft.AspNetCore.Mvc.Rendering;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services
{
    public interface ISliderService
    {
        // Lấy danh sách Slider và phân trang
        Task<(List<SliderModel> data, Paginate pager)> GetlistItemAsync(int page);
        // Tạo Slider mới
        Task<OperationResult> CreateAsync(SliderModel slider);
        // Tìm kiếm Slider
        Task<SliderModel?> FindSlidersAsync(int id);
        // Cập nhật Slider
        Task<OperationResult> UpdateAsync(SliderModel slider);
        // Xóa Slider
        Task<OperationResult> DeleteAsync(int id);

    }
}
