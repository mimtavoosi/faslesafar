using System.ComponentModel.DataAnnotations;

namespace FasleSafar.Models
{
    public class AddDestinationVM
    {
        [Required]
        [Display(Name = "کد مقصد")]
        public int DestinationId { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string DestinationName { get; set; }

        [Display(Name = "توضیحات")]
        public string? DestinationDescription { get; set; }

        [Display(Name = "کشور")]
        public string? Country { get; set; }

        [Display(Name = "استان")]
        public string? Province { get; set; }

        [Display(Name = "شهر")]
        public string? City { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }

        [Display(Name = "جاذبه گردشگری")]
        public bool IsAttraction { get; set; }

        [Display(Name = "نمایش در ویترین")]
        public bool OnVitrin { get; set; }

        [Display(Name = "تصویر (1920 * 600)")]
        [Required(ErrorMessage = "لطفا {0} تور را وارد کنید")]
        public IFormFile BigImage { get; set; }

        [Display(Name = "آلبوم تصاویر (600 * 1920)")]
        public List<IFormFile>? AlbumImages { get; set; }
    }

    public class EditDestinationVM
    {
        [Required]
        [Display(Name = "کد مقصد")]
        public int DestinationId { get; set; }

        [Display(Name = "مقصد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(60)]
        public string DestinationName { get; set; }

        [Display(Name = "توضیحات")]
        public string? DestinationDescription { get; set; }

        [Display(Name = "کشور")]
        public string? Country { get; set; }

        [Display(Name = "استان")]
        public string? Province { get; set; }

        [Display(Name = "شهر")]
        public string? City { get; set; }

        [Display(Name = "مختصات جغرافیایی")]
        public string? GeoCoordinates { get; set; }

        [Display(Name = "جاذبه گردشگری")]
        public bool IsAttraction { get; set; }

        [Display(Name = "نمایش در ویترین")]
        public bool OnVitrin { get; set; }

        [Display(Name = "تصویر بزرگ")]
        public string? BigImage { get; set; }

        [Display(Name = "آلبوم تصاویر")]
        public string? ImagesAlbum { get; set; }

        [Display(Name = "تصویر (1920 * 600)")]
        public IFormFile? BigDestinationImage { get; set; }

        [Display(Name = "آلبوم تصاویر (600 * 1920)")]
        public List<IFormFile>? AlbumImages { get; set; }
    }
}
