using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Shopping_ver1.Models;
using Shopping_ver1.Repository;
using Shopping_ver1.Services.Abstract;

public class SliderService : ISliderService
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _env;

    public SliderService(DataContext context, IWebHostEnvironment env)
    {
        _dataContext = context;
        _env = env;
    }

    // Format lại text trong Description
    private string SanitizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return string.Empty;

        var decoded = WebUtility.HtmlDecode(description);
        var noHtml = Regex.Replace(decoded, "<.*?>", string.Empty);
        return Regex.Replace(noHtml, @"\s+", " ").Trim();
    }

    // Xử lý ảnh sau đó trả về tên của ảnh đó
    public async Task<string> SaveImage(IFormFile imageUpload)
    {
        // Đảo bảo đã có file được upload
        if (imageUpload == null || imageUpload.Length == 0)
            throw new ArgumentException("Image is required");

        // Đường dẫn nới lưu trữ hình ảnh ( nếu chưa có folder đó thì tạo )
        string uploadDir = Path.Combine(_env.WebRootPath, "media/sliders");
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

    // Lấy danh sách Slider
    public async Task<List<SliderModel>> GetlistItemAsync()
    {
        return await _dataContext.Sliders.ToListAsync();
    }

    // Tạo Slider mới
    public async Task<OperationResult> CreateAsync(SliderModel slider)
    {
        try
        {
            // Chắc chắn chắn chọn ảnh và đảm bảo không phải ảnh rác hoặc lỗi 
            if (slider.ImageUpload == null || slider.ImageUpload.Length == 0)
                return new OperationResult(false, "Vui lòng chọn ảnh cho Slider");

            // Lưu ảnh
            slider.Image = await SaveImage(slider.ImageUpload);

            // Format lại text trong Description
            slider.Description = SanitizeDescription(slider.Description);

            // Tạo mới và lưu lại
            await _dataContext.Sliders.AddAsync(slider);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Thêm Slider mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Thêm Slider thất bại!!!");
        }
    }

    // Tìm kiếm Slider
    public async Task<SliderModel?> FindItemsAsync(int id)
    {
        return await _dataContext.Sliders.FindAsync(id);
    }

    // Cập nhật Slider
    public async Task<OperationResult> UpdateAsync(SliderModel slider)
    {
        try
        {
            // Chắc chắn chắn chọn ảnh và đảm bảo không phải ảnh rác hoặc lỗi 
            if (slider.ImageUpload != null)
            {
                if (slider.ImageUpload == null || slider.ImageUpload.Length == 0)
                    return new OperationResult(false, "Vui lòng chọn ảnh");

                // Xóa ảnh trong wroot
                if (slider.Image != "default.png")
                {
                    string path = Path.Combine(_env.WebRootPath, "media/sliders", slider.Image);
                    if (File.Exists(path))
                        File.Delete(path);
                }
                // Lưu ảnh
                slider.Image = await SaveImage(slider.ImageUpload);
            }
            // Format lại text trong Description
            slider.Description = SanitizeDescription(slider.Description);

            // Cập nhật và lưu lại
            _dataContext.Sliders.Update(slider);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Cập nhật Slider mới thành công!!!");
        }
        catch
        {
            return new OperationResult(false, "Cập nhật Slider thất bại!!!");
        }
    }

    // Xóa Slider
    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            // Tìm Slider
            var slider = await _dataContext.Sliders.FindAsync(id);
            if (slider == null)
                return new OperationResult(false, "Không tìm thấy Slider");

            // Xóa ảnh trong wroot
            if (slider.Image != "default.png")
            {
                string path = Path.Combine(_env.WebRootPath, "media/sliders", slider.Image);
                if (File.Exists(path))
                    File.Delete(path);
            }

            // Xóa và lưu lại
            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();

            return new OperationResult(true, "Xóa Slider thành công");

        }
        catch
        {
            return new OperationResult(false, "Xóa Slider thất bại");

        }
    }
}
