namespace Shopping_ver1.Services
{
    public interface IEmailService
    {
        // Gửi email
        Task SendEmailAsync(string email, string subject, string message);
    }
}
