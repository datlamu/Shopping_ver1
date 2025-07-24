using Shopping_ver1.Helpers;
using Shopping_ver1.Models;

namespace Shopping_ver1.Services.Abstract
{
    public interface IContactService
    {
        // Lấy danh sách thông tin liên hệ
        Task<(ContactModel data, Paginate pager)> GetlistItemAsync(int page);
        // Lấy danh sách thông tin liên hệ
        Task<List<ContactModel>> GetlistItemAsync();

        // Tạo thông tin liên hệ mới
        Task<OperationResult> CreateAsync(ContactModel contact);

        // Tìm kiếm thông tin liên hệ
        Task<ContactModel> FindItemAsync(int id);

        // Cập nhật thông tin liên hệ
        Task<OperationResult> UpdateAsync(ContactModel contact);

        // Xóa thông tin liên hệ
        Task<OperationResult> DeleteAsync(int id);
    }
}
