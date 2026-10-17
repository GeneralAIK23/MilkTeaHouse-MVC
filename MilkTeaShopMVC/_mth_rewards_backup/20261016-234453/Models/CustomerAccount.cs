using System;
using System.ComponentModel.DataAnnotations;

namespace MilkTeaShopMVC.Models
{
    public class CustomerAccount
    {
        public int Id { get; set; }
        [Required, StringLength(120), EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(100)]
        public string FullName { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [Required, StringLength(500)]
        public string PasswordHash { get; set; }
        [Required, StringLength(500)]
        public string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
