using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class Content
    {
        [Key]
        [Display(Name = "کد محتوا")]
        public int ContentId { get; set; }

        [Display(Name = "تصویر")]
        public bool HasImage { get; set; }

        [Display(Name = "نوع محتوا")]
        [Required(ErrorMessage = "لطفا {0} را انتخاب کنید")]
        [MaxLength(30)]
        public string ContentType { get; set; }

        [Display(Name = "عنوان محتوا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string ContentTitle { get; set; }

        [Display(Name = "متن محتوا")]
        public string? ContentText { get; set; }

        [Display(Name = "پسوند تصویر")]
        [MaxLength(6)]
        public string? ImageExt { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }
    }
}
