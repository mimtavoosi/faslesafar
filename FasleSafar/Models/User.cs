using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class User
    {
        [Key]
        [Display(Name = "کد کاربر")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(120)]
        [Display(Name = "نام")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(11)]
        [Display(Name = "شماره موبایل")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(20)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Display(Name = "نوع کاربر")]
        public string IsAdmin { get; set; }

        // An User have many Orders
        public List<Order>? Orders { get; set; }
        public List<ReqTrip>? ReqTrips { get; set; }
    }
}
