using System.ComponentModel.DataAnnotations;

namespace MilkTeaShopMVC.Models
{
    public class CustomerRegisterViewModel
    {
        [Required(ErrorMessage="Nhập họ và tên."), StringLength(100)]
        [Display(Name="Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage="Nhập email."), EmailAddress(ErrorMessage="Email không hợp lệ."), StringLength(120)]
        [Display(Name="Email")]
        public string Email { get; set; }

        [Required(ErrorMessage="Nhập mật khẩu."), StringLength(100, MinimumLength=8, ErrorMessage="Mật khẩu cần ít nhất 8 ký tự.")]
        [DataType(DataType.Password), Display(Name="Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage="Xác nhận mật khẩu."), DataType(DataType.Password)]
        [Compare("Password", ErrorMessage="Mật khẩu xác nhận không khớp.")]
        [Display(Name="Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }

    public class CustomerLoginViewModel
    {
        [Required, EmailAddress, Display(Name="Email")]
        public string Email { get; set; }
        [Required, DataType(DataType.Password), Display(Name="Mật khẩu")]
        public string Password { get; set; }
    }

    public class CustomerProfileViewModel
    {
        [Required, StringLength(100), Display(Name="Họ và tên")]
        public string FullName { get; set; }
        [EmailAddress, Display(Name="Email")]
        public string Email { get; set; }
        [StringLength(20), RegularExpression(@"^[0-9+(). -]{0,20}$", ErrorMessage="Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
    }
}
