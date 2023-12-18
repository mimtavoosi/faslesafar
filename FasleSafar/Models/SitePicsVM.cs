using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class SitePicsVM
    {
        [Display(Name = "لوگو")]
        public IFormFile? LogoImage { get; set; }

        [Display(Name = "آیکون (پسوند .ico)")]
        public IFormFile? IconImage { get; set; }

        [Display(Name = "تصویر پیش فرض محتوا (600 * 1920)")]
        public IFormFile? DefaultContentImage { get; set; }

        [Display(Name = "بنر اصلی (600 * 1920)")]
        public IFormFile? BannerImage { get; set; }

        //[Display(Name = "بنر لیست تور ها (200 * 1920)")]
        //public IFormFile? ToursList { get; set; }

        [Display(Name = "شماره تماس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "مقدار {0} باید 11 رقمی و فقط شامل اعداد باشد")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(600)]
        public string Address { get; set; }
        
        [Display(Name = "شعار")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(600)]
        public string Slogan { get; set; }

        [Display(Name = "درصد بیعانه")]
        public int? Deposit { get; set; }

        [Display(Name = "درصد مالیات")]
        public int? Maliat { get; set; }

        [Display(Name = "قوانین سفرهای داخلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string IranRules { get; set; } 
        [Display(Name = "قوانین سفرهای خارجی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string WorldRules { get; set; }
    }
}
