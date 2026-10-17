using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MilkTeaShopMVC.Models
{
    public class FranchiseRequestForm : IValidatableObject
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(120, ErrorMessage = "Tên tối đa 120 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^[0-9+(). -]{9,25}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(200)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập khu vực muốn hợp tác.")]
        [StringLength(160)]
        public string Region { get; set; }

        [StringLength(1500)]
        public string Message { get; set; }

        public bool AcceptPrivacy { get; set; }

        // Honeypot: leave empty for real users; not stored.
        public string Website { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AcceptPrivacy)
                yield return new ValidationResult("Bạn cần đồng ý để gửi yêu cầu.", new[] { "AcceptPrivacy" });
            if (!string.IsNullOrEmpty(Gender) && Gender != "Nam" && Gender != "Nữ" && Gender != "Khác")
                yield return new ValidationResult("Giới tính không hợp lệ.", new[] { "Gender" });
        }
    }
}
