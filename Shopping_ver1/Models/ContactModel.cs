using System.ComponentModel.DataAnnotations;

namespace Shopping_ver1.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên cửa hàng")]
        public string ShopName { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên quốc gia")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }
        public ContactModel() { }
    }
}
