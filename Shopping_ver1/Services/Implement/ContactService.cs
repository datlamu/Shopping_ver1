using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Helpers;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class ContactService : IContactService
{
    private readonly DataContext _dataContext;

    public ContactService(DataContext context)
    {
        _dataContext = context;
    }

    // Lấy src của iframe
    private string ExtractSrcFromIframe(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Nếu người dùng chỉ nhập sẵn src thì bỏ qua
        if (input.StartsWith("http"))
            return input;

        // Regex để lấy src từ iframe
        var match = Regex.Match(input, @"<iframe[^>]+src\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : input;
    }

    // Lấy danh sách thông tin liên hệ
    public async Task<List<ContactModel>> GetlistItemAsync()
    {
        return await _dataContext.Contacts.ToListAsync();
    }

    // Lấy danh sách thông tin liên hệ và phân trang
    public async Task<(ContactModel data, Paginate pager)> GetlistItemAsync(int page)
    {
        try
        {
            // Tổng số Items
            var totalItems = await _dataContext.Contacts.CountAsync();
            // Tạo đối tượng phân trang
            var pager = new Paginate(totalItems, page, 1);

            // Danh sách items
            var data = await _dataContext.Contacts
                .Skip(pager.Skip)       // Bỏ qua số lượng phần tử
                .Take(1)   // Lấy số lượng phần tử tiếp đó
                .FirstOrDefaultAsync();

            return (data, pager);
        }
        catch
        {
            return (new ContactModel(), new Paginate());
        }
    }

    // Tạo thông tin liên hệ mới
    public async Task<OperationResult> CreateAsync(ContactModel contact)
    {
        try
        {
            // Lấy src của iframe
            contact.Map = ExtractSrcFromIframe(contact.Map);

            // Tạo mới và lưu lại
            await _dataContext.Contacts.AddAsync(contact);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm thông tin liên hệ mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm thông tin liên hệ mới thất bại!!!");
        }
    }

    // Tìm kiếm thông tin liên hệ
    public async Task<ContactModel> FindItemAsync(int id)
    {
        return await _dataContext.Contacts.FindAsync(id);
    }

    // Cập nhật thông tin liên hệ
    public async Task<OperationResult> UpdateAsync(ContactModel contact)
    {
        try
        {
            // Lấy src của iframe
            contact.Map = ExtractSrcFromIframe(contact.Map);

            // Cập nhật và lưu lại
            _dataContext.Contacts.Update(contact);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật thông tin liên hệ thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật thông tin liên hệ thất bại!!!");
        }
    }

    // Xóa thông tin liên hệ
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm thông tin liên hệ
            var contact = await _dataContext.Contacts.FindAsync(id);
            if (contact == null)
                return new OperationResult(false, "Không tìm thấy thông tin liên hệ");

            // Xóa và lưu lại
            _dataContext.Contacts.Remove(contact);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa thông tin liên hệ thành công");

        }
        catch
        {
            return new OperationResult(false, "Xóa thông tin liên hệ thất bại");

        }
    }
}
