using Microsoft.Extensions.Options;
using Shopping_ver1.Models;
using System.Net.Mail;
using System.Net;
using Shopping_ver1.Services.Abstract;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    // Gửi Email
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Tạo SmtpClient kết nối đến máy chủ SMTP (ở đây là Gmail) với host và port lấy từ cấu hình EmailSettings
        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = true, // Bật mã hóa SSL
            UseDefaultCredentials = false, // Tắt xác minh thông tin từ hệ thống
            Credentials = new NetworkCredential(_settings.Email, _settings.Password) // Thông tin đăng nhập SMTP
        };

        // Tạo MailMessage đại diện cho email cần gửi
        using var mail = new MailMessage(_settings.Email, toEmail, subject, body)
        {
            IsBodyHtml = true // Nội dung email được gửi dưới dạng HTML
        };

        // Gửi email
        await client.SendMailAsync(mail);
    }
}