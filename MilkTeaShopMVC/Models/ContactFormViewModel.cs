using System.ComponentModel.DataAnnotations;

namespace MilkTeaShopMVC.Models
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Họ tên cần từ 2 đến 120 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email chưa hợp lệ.")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại chưa hợp lệ.")]
        [StringLength(25)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [StringLength(250)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ.")]
        [StringLength(3000, MinimumLength = 10, ErrorMessage = "Nội dung cần từ 10 đến 3000 ký tự.")]
        [Display(Name = "Nội dung")]
        public string Message { get; set; }

        public bool Consent { get; set; }

        // Honeypot chống bot tự động đơn giản; không hiển thị với người dùng.
        public string Website { get; set; }
    }
}
