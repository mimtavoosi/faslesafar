using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class AddEditUserVM
    {
        [Required]
        [Display(Name = "کد کاربر")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(200)]
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "پست الکترونیک معتبر نیست")]
		[Display(Name = "پست الکترونیک")]
        [Remote("isNewEmailAddress", "Admin", AdditionalFields = "UserId")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید ")]
        [MaxLength(120)]
        [Display(Name = "نام")]
        public string FullName { get; set; }
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} شخص را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        [Remote("isNewMobileNumber", "Admin", AdditionalFields = "UserId")]
        public string MobileNumber { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(20)]
        [DataType(DataType.Password)] //Hide Characters
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "کلمه عبور باید شامل حرف و عدد باشد")] //check exist number & alphabet chars in password field
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)] //Hide Characters
        [Compare("Password", ErrorMessage = "کلمه عبور و تکرار کلمه عبور یکسان نیستند")] //Compare value with Password field
        [MaxLength(20)]
        public string RePassword { get; set; }

        [Display(Name = "نوع کاربر")]
        public string? IsAdmin { get; set; }
    }
}
