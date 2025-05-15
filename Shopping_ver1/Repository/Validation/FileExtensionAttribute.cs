using System.ComponentModel.DataAnnotations;

public class FileExtensionAttribute : ValidationAttribute
{
    // Ghi đè phương thức kiểm tra tính hợp lệ
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext) // Thêm ? để chỉ rõ value có thể là null
    {
        // Kiểm tra nếu value là IFormFile và không null
        if (value is IFormFile file)
        {
            // Lấy phần mở rộng của file và chuyển về chữ thường (vd: .png)
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            // Đuôi file hợp lệ
            string[] extensions = { ".jpg", ".png", ".jpeg" };

            // Kiểm tra file
            bool result = extensions.Contains(extension);
            if (!result)
                return new ValidationResult("Allowed extensions are jpg, png or jpeg");
        }

        return ValidationResult.Success;
    }
}
