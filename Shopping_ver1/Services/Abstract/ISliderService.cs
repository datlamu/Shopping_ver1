using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface ISliderService
    {
        // Lấy danh sách Slider và phân trang
        Task<List<SliderModel>> GetlistItemAsync();
        // Tạo Slider mới
        Task<OperationResult> CreateAsync(SliderModel slider);
        // Tìm kiếm Slider
        Task<SliderModel> FindItemsAsync(int id);
        // Cập nhật Slider
        Task<OperationResult> UpdateAsync(SliderModel slider);
        // Xóa Slider
        Task<OperationResult> DeleteAsync(int id);

    }
}
