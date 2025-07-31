using System.ComponentModel.DataAnnotations;

public class NewPasswordViewModel
{
    public string Email { get; set; }
    public string Token { get; set; }

    [Required(ErrorMessage = "Hãy nhập mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Hãy nhập lại mật khẩu")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
    public string RePassword { get; set; }
}
