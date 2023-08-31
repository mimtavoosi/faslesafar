using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class EditStateVM
    {
        [Required]
        [Display(Name = "کد سفارش")]
        public int OrderId { get; set; }

        [Display(Name = "نام کاربر")]
        public string? UserName { get; set; }
        [Required]
        [Display(Name = "وضعیت سفارش")]
        public string PayState { get; set; }
    }
}
